using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class ProxyResponseWriter : IResponseWriter
    {
        private static readonly string[] _excludedRequestHeaderNames = {"content-type", "content-length", "host"};
        private static readonly string[] _excludedResponseHeaderNames = {"x-httplaceholder-correlation", "transfer-encoding"};
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
            if (string.IsNullOrWhiteSpace(stub.Response?.ProxyToUrl))
            {
                return false;
            }

            // TODO
            // - Add post data: plain text
            // - Add post data: complex (e.g. file upload)
            // - Do a web call and add result body and headers to stub response.
            // - SSL validation (enable / disable?)
            // - Proxy exact path as received by HttPlaceholder or the URL as configured in the stub.
            using var httpClient = _httpClientFactory.CreateClient();
            var method = new HttpMethod(_httpContextService.Method);
            var request = new HttpRequestMessage(method, stub.Response.ProxyToUrl);
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
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentTypeHeader.Value);
                    }
                }
            }

            using var responseMessage = await httpClient.SendAsync(request);
            var content = await responseMessage.Content.ReadAsByteArrayAsync();
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
