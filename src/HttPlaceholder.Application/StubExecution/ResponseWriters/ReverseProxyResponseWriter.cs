using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to setup a reverse proxy to another URL.
/// </summary>
internal class ReverseProxyResponseWriter : IResponseWriter
{
    private static readonly string[] _excludedRequestHeaderNames =
    {
        "content-type", "content-length", "host", "connection", "accept-encoding"
    };

    private static readonly string[] _excludedResponseHeaderNames =
    {
        "x-httplaceholder-correlation", "transfer-encoding", "content-length"
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
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (stub.Response.ReverseProxy == null || string.IsNullOrWhiteSpace(stub.Response.ReverseProxy.Url))
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        var proxyUrl = stub.Response.ReverseProxy.Url;
        var appendPath = stub.Response.ReverseProxy.AppendPath == true;
        if (appendPath)
        {
            proxyUrl = proxyUrl.EnsureEndsWith("/") + _httpContextService.Path.TrimStart('/');
            var path = GetPath(stub);
            if (!string.IsNullOrWhiteSpace(path))
            {
                // If the path condition is set, make sure the configured path is stripped from the proxy URL
                var index = proxyUrl.IndexOf(path, StringComparison.OrdinalIgnoreCase);
                if (index > -1)
                {
                    proxyUrl = proxyUrl.Remove(index, path.Length);
                }
            }
        }

        if (stub.Response.ReverseProxy.AppendQueryString == true)
        {
            proxyUrl += _httpContextService.GetQueryString();
        }

        var method = new HttpMethod(_httpContextService.Method);
        var request = new HttpRequestMessage(method, proxyUrl);
        var log = $"Performing {method} request to URL {proxyUrl}";
        var originalHeaders = _httpContextService
            .GetHeaders();
        var headers = originalHeaders
            .Where(h => !_excludedRequestHeaderNames.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .ToArray();
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        if (method != HttpMethod.Get)
        {
            var requestBody = _httpContextService.GetBodyAsBytes();
            if (requestBody.Any())
            {
                request.Content = new ByteArrayContent(requestBody);
                var contentTypeHeader = originalHeaders.FirstOrDefault(h =>
                    h.Key.Equals("content-type", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(contentTypeHeader.Value))
                {
                    request.Content.Headers.Add("Content-Type", contentTypeHeader.Value);
                }
            }
        }

        using var httpClient = _httpClientFactory.CreateClient("proxy");
        using var responseMessage = await httpClient.SendAsync(request);
        var content = responseMessage.Content != null
            ? await responseMessage.Content.ReadAsByteArrayAsync()
            : Array.Empty<byte>();
        var rawResponseHeaders = responseMessage.Headers
            .ToDictionary(h => h.Key, h => h.Value.First());
        if (stub.Response.ReverseProxy.ReplaceRootUrl == true)
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

            rawResponseHeaders = rawResponseHeaders
                .ToDictionary(h => h.Key, h => h.Value.Replace(rootUrl, httPlaceholderRootUrl));
        }

        response.Body = content;
        var contentHeaders = responseMessage.Content != null
            ? responseMessage.Content.Headers.ToDictionary(h => h.Key, h => h.Value.First())
            : new Dictionary<string, string>();
        var responseHeaders = rawResponseHeaders
            .Concat(contentHeaders)
            .Where(h => !_excludedResponseHeaderNames.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .ToArray();
        foreach (var header in responseHeaders)
        {
            response.Headers.AddOrReplaceCaseInsensitive(header.Key, header.Value);
        }

        response.StatusCode = (int)responseMessage.StatusCode;
        return StubResponseWriterResultModel.IsExecuted(GetType().Name, log);
    }

    private string GetPath(StubModel stub)
    {
        var pathModel = stub.Conditions?.Url?.Path;
        if (pathModel is string path)
        {
            return path;
        }

        if (pathModel is StubConditionStringCheckingModel checkingModel)
        {
            return checkingModel.StringEquals ??
                   checkingModel.StringEqualsCi ?? checkingModel.StartsWith ?? checkingModel.StartsWithCi;
        }

        return null;
    }
}
