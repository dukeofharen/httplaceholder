using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Metadata;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Dto.Users;
using HttPlaceholder.Client.Exceptions;
using Newtonsoft.Json;

namespace HttPlaceholder.Client.Implementations
{
    /// <inheritdoc />
    public class HttPlaceholderClient : IHttPlaceholderClient
    {
        private const string JsonContentType = "application/json";
        private readonly HttpClient _httpClient;

        public HttPlaceholderClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<MetadataDto> GetMetadataAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/metadata");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<MetadataDto>(content);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RequestResultDto>> GetAllRequestsAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/requests");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<RequestResultDto>>(content);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RequestOverviewDto>> GetRequestOverviewAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/requests/overview");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<RequestOverviewDto>>(content);
        }

        /// <inheritdoc />
        public async Task<RequestResultDto> GetRequestAsync(string correlationId)
        {
            using var response = await _httpClient.GetAsync($"/ph-api/requests/{correlationId}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<RequestResultDto>(content);
        }

        /// <inheritdoc />
        public async Task DeleteAllRequestsAsync()
        {
            using var response = await _httpClient.DeleteAsync("/ph-api/requests");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task<FullStubDto> CreateStubForRequestAsync(string correlationId)
        {
            using var response =
                await _httpClient.PostAsync($"/ph-api/requests/{correlationId}/stubs", new StringContent(""));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<FullStubDto>(content);
        }

        /// <inheritdoc />
        public async Task<FullStubDto> CreateStubAsync(StubDto stub)
        {
            using var response =
                await _httpClient.PostAsync($"/ph-api/stubs",
                    new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, JsonContentType));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<FullStubDto>(content);
        }

        /// <inheritdoc />
        public async Task UpdateStubAsync(StubDto stub, string stubId)
        {
            using var response =
                await _httpClient.PutAsync($"/ph-api/stubs/{stubId}",
                    new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, JsonContentType));
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FullStubDto>> GetAllStubsAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/stubs");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FullStubOverviewDto>> GetStubOverviewAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/stubs/overview");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<FullStubOverviewDto>>(content);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RequestResultDto>> GetRequestsByStubIdAsync(string stubId)
        {
            using var response = await _httpClient.GetAsync($"/ph-api/stubs/{stubId}/requests");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<RequestResultDto>>(content);
        }

        /// <inheritdoc />
        public async Task<FullStubDto> GetStubAsync(string stubId)
        {
            using var response = await _httpClient.GetAsync($"/ph-api/stubs/{stubId}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<FullStubDto>(content);
        }

        /// <inheritdoc />
        public async Task DeleteStubAsync(string stubId)
        {
            using var response = await _httpClient.DeleteAsync($"/ph-api/stubs/{stubId}");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAllStubsAsync()
        {
            using var response = await _httpClient.DeleteAsync("/ph-api/stubs");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetTenantNamesAsync()
        {
            using var response = await _httpClient.GetAsync("/ph-api/tenants");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<string>>(content);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FullStubDto>> GetStubsByTenantAsync(string tenant)
        {
            using var response = await _httpClient.GetAsync($"/ph-api/tenants/{tenant}/stubs");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
        }

        /// <inheritdoc />
        public async Task DeleteAllStubsByTenantAsync(string tenant)
        {
            using var response = await _httpClient.DeleteAsync($"/ph-api/tenants/{tenant}/stubs");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubDto> stubs)
        {
            using var response = await _httpClient.PutAsync($"/ph-api/tenants/{tenant}/stubs",
                new StringContent(JsonConvert.SerializeObject(stubs), Encoding.UTF8, JsonContentType));
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task<UserDto> GetUserAsync(string username)
        {
            using var response = await _httpClient.GetAsync($"/ph-api/users/{username}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<UserDto>(content);
        }
    }
}
