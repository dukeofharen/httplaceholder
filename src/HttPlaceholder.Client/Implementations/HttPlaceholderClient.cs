using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Metadata;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Exceptions;
using Newtonsoft.Json;

namespace HttPlaceholder.Client.Implementations
{
    public class HttPlaceholderClient : IHttPlaceholderClient
    {
        private readonly HttpClient _httpClient;

        public HttPlaceholderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<MetadataDto> GetMetadataAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/metadata");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(
                    $"Status code '{(int)response.StatusCode}' returned by HttPlaceholder with message '{content}'");
            }

            return JsonConvert.DeserializeObject<MetadataDto>(content);
        }

        public async Task<IEnumerable<RequestResultDto>> GetAllRequestsAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/requests");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(
                    $"Status code '{(int)response.StatusCode}' returned by HttPlaceholder with message '{content}'");
            }

            return JsonConvert.DeserializeObject<IEnumerable<RequestResultDto>>(content);
        }

        public async Task<IEnumerable<RequestOverviewDto>> GetRequestOverviewAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/requests/overview");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(
                    $"Status code '{(int)response.StatusCode}' returned by HttPlaceholder with message '{content}'");
            }

            return JsonConvert.DeserializeObject<IEnumerable<RequestOverviewDto>>(content);
        }
    }
}
