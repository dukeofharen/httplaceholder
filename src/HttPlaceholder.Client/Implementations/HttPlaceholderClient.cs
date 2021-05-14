using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Metadata;

namespace HttPlaceholder.Client.Implementations
{
    public class HttPlaceholderClient : IHttPlaceholderClient
    {
        private readonly HttpClient _httpClient;

        public HttPlaceholderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task<MetadataDto> GetMetadataAsync() => throw new System.NotImplementedException();
    }
}
