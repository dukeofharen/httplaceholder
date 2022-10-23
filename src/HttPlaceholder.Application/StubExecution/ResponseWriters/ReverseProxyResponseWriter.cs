using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution.Utilities;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to setup a reverse proxy to another URL.
/// </summary>
internal class ReverseProxyResponseWriter : IResponseWriter, ISingletonService
{
    private static readonly string[] _excludedRequestHeaderNames =
    {
        HeaderKeys.ContentType, HeaderKeys.ContentLength, HeaderKeys.Host, HeaderKeys.Connection,
        HeaderKeys.AcceptEncoding
    };

    private static readonly string[] _excludedResponseHeaderNames =
    {
        HeaderKeys.XHttPlaceholderCorrelation, HeaderKeys.XHttPlaceholderExecutedStub, HeaderKeys.TransferEncoding,
        HeaderKeys.ContentLength
    };

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextService _httpContextService;

    public ReverseProxyResponseWriter(
        IHttpClientFactory httpClientFactory,
        IHttpContextService httpContextService)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextService = httpContextService;
    }

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
            proxyUrl += _httpContextService.GetQueryString();
        }

        var method = new HttpMethod(_httpContextService.Method);
        var request = new HttpRequestMessage(method, proxyUrl);
        var log = $"Performing {method} request to URL {proxyUrl}";
        var originalHeaders = _httpContextService.GetHeaders();
        var headers = CleanHeaders(originalHeaders);
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        if (method != HttpMethod.Get)
        {
            await AddRequestBodyAsync(request, originalHeaders, cancellationToken);
        }

        using var httpClient = _httpClientFactory.CreateClient("proxy");
        using var responseMessage = await httpClient.SendAsync(request, cancellationToken);
        var content = responseMessage.Content != null
            ? await responseMessage.Content.ReadAsByteArrayAsync()
            : Array.Empty<byte>();
        var rawResponseHeaders = responseMessage.Headers
            .ToDictionary(h => h.Key, h => h.Value.First());
        if (stub.Response.ReverseProxy.ReplaceRootUrl == true)
        {
            var replacedContent = Content(stub, content, proxyUrl, appendPath, rawResponseHeaders);
            content = replacedContent.Content;
            rawResponseHeaders = replacedContent.RawResponseHeaders;
        }

        response.Body = content;
        var responseHeaders = GetResponseHeaders(responseMessage, rawResponseHeaders);
        foreach (var header in responseHeaders)
        {
            response.Headers.AddOrReplaceCaseInsensitive(header.Key, header.Value);
        }

        response.StatusCode = (int)responseMessage.StatusCode;
        return StubResponseWriterResultModel.IsExecuted(GetType().Name, log);
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

    private (byte[] Content, Dictionary<string, string> RawResponseHeaders) Content(StubModel stub, byte[] content,
        string proxyUrl, bool appendPath,
        Dictionary<string, string> rawResponseHeaders)
    {
        var contentAsString = Encoding.UTF8.GetString(content);
        var rootUrlParts = proxyUrl.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
        var rootUrl = $"{rootUrlParts[0]}//{rootUrlParts[1]}";
        var httPlaceholderRootUrl = _httpContextService.RootUrl;
        var path = GetPath(stub);
        if (appendPath && !string.IsNullOrWhiteSpace(path))
        {
            httPlaceholderRootUrl += path.EnsureStartsWith("/");
        }

        contentAsString = contentAsString.Replace(rootUrl, httPlaceholderRootUrl);
        content = Encoding.UTF8.GetBytes(contentAsString);
        return (content, rawResponseHeaders
            .ToDictionary(h => h.Key, h => h.Value.Replace(rootUrl, httPlaceholderRootUrl)));
    }

    private async Task AddRequestBodyAsync(HttpRequestMessage request, IDictionary<string, string> originalHeaders,
        CancellationToken cancellationToken)
    {
        var requestBody = await _httpContextService.GetBodyAsBytesAsync(cancellationToken);
        if (!requestBody.Any())
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
        proxyUrl = proxyUrl.EnsureEndsWith("/") + _httpContextService.Path.TrimStart('/');
        var path = GetPath(stub);
        if (string.IsNullOrWhiteSpace(path))
        {
            return proxyUrl;
        }

        // If the path condition is set, make sure the configured path is stripped from the proxy URL
        var index = proxyUrl.IndexOf(path, StringComparison.OrdinalIgnoreCase);
        if (index > -1)
        {
            proxyUrl = proxyUrl.Remove(index, path.Length);
        }

        return proxyUrl;
    }

    private static string GetPath(StubModel stub)
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
