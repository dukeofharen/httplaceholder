using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to setup a reverse proxy to another URL.
/// </summary>
internal class ReverseProxyResponseWriter(
    IHttpClientFactory httpClientFactory,
    IHttpContextService httpContextService,
    ILogger<ReverseProxyResponseWriter> logger,
    IUrlResolver urlResolver)
    : IResponseWriter, ISingletonService
{
    private static readonly string[] _excludedRequestHeaderNames =
    [
        HeaderKeys.ContentType, HeaderKeys.ContentLength, HeaderKeys.Host, HeaderKeys.Connection,
        HeaderKeys.AcceptEncoding
    ];

    private static readonly string[] _excludedResponseHeaderNames =
    [
        HeaderKeys.XHttPlaceholderCorrelation, HeaderKeys.XHttPlaceholderExecutedStub, HeaderKeys.TransferEncoding,
        HeaderKeys.ContentLength
    ];

    /// <inheritdoc />
    public int Priority => -10;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        if (stub.Response.ReverseProxy == null || string.IsNullOrWhiteSpace(stub.Response.ReverseProxy.Url))
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        var proxyUrl = stub.Response.ReverseProxy.Url;
        var appendPath = stub.Response.ReverseProxy.AppendPath == true;
        if (appendPath)
        {
            proxyUrl = AppendPath(stub, proxyUrl);
        }

        if (stub.Response.ReverseProxy.AppendQueryString == true)
        {
            proxyUrl += httpContextService.GetQueryString();
        }

        var method = new HttpMethod(httpContextService.Method);
        var request = new HttpRequestMessage(method, proxyUrl);
        var log = new StringBuilder();
        log.AppendLine($"Performing {method} request to URL {proxyUrl}");
        var originalHeaders = httpContextService.GetHeaders();
        var headers = CleanHeaders(originalHeaders);
        foreach (var header in headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (method != HttpMethod.Get)
        {
            await AddRequestBodyAsync(request, originalHeaders, cancellationToken);
        }

        using var httpClient = httpClientFactory.CreateClient("proxy");
        try
        {
            using var responseMessage = await httpClient.SendAsync(request, cancellationToken);
            var content = await responseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
            var rawResponseHeaders = responseMessage.Headers
                .ToDictionary(h => h.Key, h => h.Value.First());
            var isBinary = !content.IsValidAscii();
            if (stub.Response.ReverseProxy.ReplaceRootUrl == true)
            {
                var replacedContent =
                    ReplaceUrlInContent(stub, content, isBinary, proxyUrl, appendPath, rawResponseHeaders);
                content = replacedContent.Content;
                rawResponseHeaders = replacedContent.RawResponseHeaders;
            }

            response.Body = content;
            response.BodyIsBinary = isBinary;
            var responseHeaders = GetResponseHeaders(responseMessage, rawResponseHeaders);
            foreach (var header in responseHeaders)
            {
                response.Headers.AddOrReplaceCaseInsensitive(header.Key, header.Value);
            }

            response.StatusCode = (int)responseMessage.StatusCode;
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, $"Exception occurred while calling URL {proxyUrl}");
            log.AppendLine($"Exception occurred while calling URL {proxyUrl}: {ex.Message}");
            response.Body = "502 Bad Gateway"u8.ToArray();
            response.StatusCode = (int)HttpStatusCode.BadGateway;
            response.Headers.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, MimeTypes.TextMime);
            response.BodyIsBinary = false;
        }

        return StubResponseWriterResultModel.IsExecuted(GetType().Name, log.ToString());
    }

    private static IDictionary<string, string> GetResponseHeaders(HttpResponseMessage responseMessage,
        Dictionary<string, string> rawResponseHeaders)
    {
        var contentHeaders = responseMessage.Content != null
            ? responseMessage.Content.Headers.ToDictionary(h => h.Key, h => h.Value.First())
            : new Dictionary<string, string>();
        return rawResponseHeaders
            .Concat(contentHeaders)
            .Where(h => !_excludedResponseHeaderNames.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(h => h.Key, h => h.Value);
    }

    private (byte[] Content, Dictionary<string, string> RawResponseHeaders) ReplaceUrlInContent(
        StubModel stub,
        byte[] content,
        bool isBinary,
        string proxyUrl,
        bool appendPath,
        Dictionary<string, string> rawResponseHeaders)
    {
        var rootUrlParts = proxyUrl.Split(["/"], StringSplitOptions.RemoveEmptyEntries);
        var rootUrl = $"{rootUrlParts[0]}//{rootUrlParts[1]}";
        var httPlaceholderRootUrl = urlResolver.GetRootUrl();
        var path = GetPath(stub);
        if (appendPath && !string.IsNullOrWhiteSpace(path))
        {
            httPlaceholderRootUrl += path.EnsureStartsWith("/");
        }

        if (!isBinary)
        {
            var contentAsString = Encoding.UTF8.GetString(content).Replace(rootUrl, httPlaceholderRootUrl);
            content = Encoding.UTF8.GetBytes(contentAsString);
        }

        return (content, rawResponseHeaders
            .ToDictionary(h => h.Key, h => h.Value.Replace(rootUrl, httPlaceholderRootUrl)));
    }

    private async Task AddRequestBodyAsync(HttpRequestMessage request, IDictionary<string, string> originalHeaders,
        CancellationToken cancellationToken)
    {
        var requestBody = await httpContextService.GetBodyAsBytesAsync(cancellationToken);
        if (requestBody.Length == 0)
        {
            return;
        }

        request.Content = new ByteArrayContent(requestBody);
        var contentTypeHeader = originalHeaders.CaseInsensitiveSearchPair(HeaderKeys.ContentType);
        if (!string.IsNullOrWhiteSpace(contentTypeHeader.Value))
        {
            request.Content.Headers.Add(HeaderKeys.ContentType, contentTypeHeader.Value);
        }
    }

    private static KeyValuePair<string, string>[] CleanHeaders(IDictionary<string, string> originalHeaders) =>
        originalHeaders
            .Where(h => !_excludedRequestHeaderNames.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .ToArray();

    private string AppendPath(StubModel stub, string proxyUrl)
    {
        proxyUrl = proxyUrl.EnsureEndsWith("/") + httpContextService.Path.TrimStart('/');
        var path = GetPath(stub);
        if (string.IsNullOrWhiteSpace(path))
        {
            return proxyUrl;
        }

        // If the path condition is set, make sure the configured path is stripped from the proxy URL
        var index = proxyUrl.LastIndexOf(path, StringComparison.OrdinalIgnoreCase);
        if (index > -1)
        {
            proxyUrl = proxyUrl.Remove(index, path.Length);
        }

        return proxyUrl;
    }

    internal static string GetPath(StubModel stub)
    {
        var pathModel = stub.Conditions?.Url?.Path;
        switch (pathModel)
        {
            case null:
                return null;
            case string path:
                return path;
            default:
            {
                var checkingModel = ConversionUtilities.Convert<StubConditionStringCheckingModel>(pathModel);
                return checkingModel.StringEquals ??
                       checkingModel.StringEqualsCi ??
                       checkingModel.StartsWith ??
                       checkingModel.StartsWithCi;
            }
        }
    }
}
