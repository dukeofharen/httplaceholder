using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class ProxyResponseWriter : IResponseWriter
    {
        private static readonly string[] _excludedRequestHeaderNames = {"content-type", "content-length", "host"};

        private static readonly string[] _excludedResponseHeaderNames =
        {
            "x-httplaceholder-correlation", "transfer-encoding"
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextService _httpContextService;

        public ProxyResponseWriter(
            IHttpClientFactory httpClientFactory,
            IHttpContextService httpContextService)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextService = httpContextService;
        }

        public int Priority => -10;

        public async Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (stub.Response.Proxy == null ||
                (string.IsNullOrWhiteSpace(stub.Response.Proxy.Url) &&
                 string.IsNullOrWhiteSpace(stub.Response.Proxy.BaseUrl)))
            {
                return false;
            }

            string proxyUrl;
            if (!string.IsNullOrWhiteSpace(stub.Response.Proxy.Url))
            {
                proxyUrl = stub.Response.Proxy.Url;
                if (stub.Response.Proxy.AppendQueryString == true)
                {
                    proxyUrl += _httpContextService.GetQueryString();
                }
            }
            else
            {
                var rootUrl = stub.Response.Proxy.BaseUrl.EnsureEndsWith("/");
                proxyUrl = rootUrl + _httpContextService.Path.TrimStart('/') + _httpContextService.GetQueryString();
            }

            // TODO
            // - In response, replace proxy URL with URL of HttPlaceholder.
            using var httpClient = _httpClientFactory.CreateClient("proxy");
            var method = new HttpMethod(_httpContextService.Method);
            var request = new HttpRequestMessage(method, proxyUrl);
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

            using var responseMessage = await httpClient.SendAsync(request);
            var content = await responseMessage.Content.ReadAsByteArrayAsync();
            if (stub.Response.Proxy.ReplaceRootUrl == true)
            {
                var contentAsString = Encoding.UTF8.GetString(content);
                var rootUrlParts = proxyUrl.Split(new[]{"/"}, StringSplitOptions.RemoveEmptyEntries);
                var rootUrl = $"{rootUrlParts[0]}//{rootUrlParts[1]}";
                contentAsString = contentAsString.Replace(rootUrl, _httpContextService.RootUrl);
                content = Encoding.UTF8.GetBytes(contentAsString);
            }

            response.Body = content;
            var responseHeaders = responseMessage.Headers
                .Where(h => !_excludedResponseHeaderNames.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                .ToArray();
            foreach (var header in responseHeaders)
            {
                response.Headers.Add(header.Key, header.Value.First());
            }

            response.StatusCode = (int)responseMessage.StatusCode;

            return true;
        }
    }
}
