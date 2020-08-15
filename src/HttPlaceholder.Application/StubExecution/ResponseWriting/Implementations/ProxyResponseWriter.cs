using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class ProxyResponseWriter : IResponseWriter
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProxyResponseWriter(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public int Priority => 0;

        public async Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            if (string.IsNullOrWhiteSpace(stub.Response?.ProxyToUrl))
            {
                return false;
            }

            var httpClient = _httpClientFactory.CreateClient();
            return true;
        }
    }
}
