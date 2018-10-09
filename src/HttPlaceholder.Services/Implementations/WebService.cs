using System.Net.Http;
using System.Threading.Tasks;

namespace HttPlaceholder.Services.Implementations
{
    internal class WebService : IWebService
    {
        private static HttpClient _httpClient = new HttpClient();

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message)
        {
            return await _httpClient.SendAsync(message);
        }
    }
}
