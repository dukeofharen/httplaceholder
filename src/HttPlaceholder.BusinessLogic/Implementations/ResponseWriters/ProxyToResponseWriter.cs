using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Models;
using HttPlaceholder.Services;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    public class ProxyToResponseWriter : IResponseWriter
    {
        private const string ContentLengthKey = "Content-Length";
        private const string ContentTypeKey = "Content-Type";

        private static string[] _headersToExcludeFromRequest = new[]
        {
            ContentLengthKey,
            ContentTypeKey,
            "Host"
        };
        private static string[] _headersToExcludeFromResponse = new[]
        {
            "X-HttPlaceholder-Correlation"
        };

        private readonly IHttpContextService _httpContextService;
        private readonly IWebService _webService;

        public ProxyToResponseWriter(
            IHttpContextService httpContextService,
            IWebService webService)
        {
            _httpContextService = httpContextService;
            _webService = webService;
        }

        public async Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            bool executed = false;
            if (stub.Response?.ProxyTo != null)
            {
                string method = _httpContextService.Method;
                var body = _httpContextService.GetBodyAsBytes();
                var headers = _httpContextService.GetHeaders();
                var headersForProxyRequest = headers
                 .Where(h => !_headersToExcludeFromRequest.Contains(h.Key, StringComparer.OrdinalIgnoreCase));
                var message = new HttpRequestMessage
                {
                    RequestUri = new Uri(stub.Response.ProxyTo),
                    Method = new HttpMethod(method)
                };
                if (body?.Any() == true)
                {
                    message.Content = new ByteArrayContent(body);
                    if (headers.TryGetValue(ContentTypeKey, out string contentType))
                    {
                        message.Content.Headers.Add(ContentTypeKey, contentType);
                    }

                    if (headers.TryGetValue(ContentLengthKey, out string contentLength))
                    {
                        message.Content.Headers.Add(ContentLengthKey, contentLength);
                    }
                }

                foreach (var header in headersForProxyRequest)
                {
                    message.Headers.Add(header.Key, header.Value);
                }

                var webResponse = await _webService.SendAsync(message);
                response.Body = await webResponse.Content.ReadAsByteArrayAsync();
                // TODO support multiple headers with the same key.
                response.Headers = webResponse.Headers
                    .Where(h => !_headersToExcludeFromResponse.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
                    .ToDictionary(h => h.Key, h => h.Value.First());
                response.StatusCode = (int)webResponse.StatusCode;

                executed = true;
            }

            return executed;
        }
    }
}
