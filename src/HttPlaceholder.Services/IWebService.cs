using System.Net.Http;
using System.Threading.Tasks;

namespace HttPlaceholder.Services
{
    public interface IWebService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message);
    }
}
