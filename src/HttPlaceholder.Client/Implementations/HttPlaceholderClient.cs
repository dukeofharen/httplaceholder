using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Enums;
using HttPlaceholder.Client.Dto.Metadata;
using HttPlaceholder.Client.Dto.Requests;
using HttPlaceholder.Client.Dto.Scenarios;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.Dto.Users;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.StubBuilders;
using Newtonsoft.Json;

namespace HttPlaceholder.Client.Implementations
{
    /// <inheritdoc />
    public class HttPlaceholderClient : IHttPlaceholderClient
    {
        private const string JsonContentType = "application/json";
        private const string TextContentType = "text/plain";

        public HttPlaceholderClient(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        internal HttpClient HttpClient { get; }

        /// <inheritdoc />
        public async Task<MetadataDto> GetMetadataAsync()
        {
            using var response = await HttpClient.GetAsync("/ph-api/metadata");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<MetadataDto>(content);
        }

        /// <inheritdoc />
        public async Task<bool> CheckFeatureAsync(FeatureFlagType featureFlag)
        {
            using var response =
                await HttpClient.GetAsync($"/ph-api/metadata/features/{featureFlag.ToString().ToLower()}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            var result = JsonConvert.DeserializeObject<FeatureResultDto>(content);
            return result is { Enabled: true };
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RequestResultDto>> GetAllRequestsAsync()
        {
            using var response = await HttpClient.GetAsync("/ph-api/requests");
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
            using var response = await HttpClient.GetAsync("/ph-api/requests/overview");
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
            using var response = await HttpClient.GetAsync($"/ph-api/requests/{correlationId}");
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
            using var response = await HttpClient.DeleteAsync("/ph-api/requests");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task DeleteRequestAsync(string correlationId)
        {
            using var response =
                await HttpClient.DeleteAsync($"/ph-api/requests/{correlationId}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task<FullStubDto> CreateStubForRequestAsync(string correlationId,
            CreateStubForRequestInputDto input = null)
        {
            var body = input == null ? "{}" : JsonConvert.SerializeObject(input);
            using var response =
                await HttpClient.PostAsync($"/ph-api/requests/{correlationId}/stubs",
                    new StringContent(body, Encoding.UTF8, JsonContentType));
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
                await HttpClient.PostAsync("/ph-api/stubs",
                    new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, JsonContentType));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<FullStubDto>(content);
        }

        /// <inheritdoc />
        public Task<FullStubDto> CreateStubAsync(StubBuilder stubBuilder) => CreateStubAsync(stubBuilder.Build());

        /// <inheritdoc />
        public async Task<IEnumerable<FullStubDto>> CreateStubsAsync(IEnumerable<StubDto> stubs)
        {
            using var response =
                await HttpClient.PostAsync("/ph-api/stubs/multiple",
                    new StringContent(JsonConvert.SerializeObject(stubs), Encoding.UTF8, JsonContentType));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
        }

        /// <inheritdoc />
        public Task<IEnumerable<FullStubDto>> CreateStubsAsync(IEnumerable<StubBuilder> stubs) =>
            CreateStubsAsync(stubs.Select(s => s.Build()));

        /// <inheritdoc />
        public Task<IEnumerable<FullStubDto>> CreateStubsAsync(params StubDto[] stubs) =>
            CreateStubsAsync(stubs.AsEnumerable());

        /// <inheritdoc />
        public Task<IEnumerable<FullStubDto>> CreateStubsAsync(params StubBuilder[] stubs) =>
            CreateStubsAsync(stubs.AsEnumerable());

        /// <inheritdoc />
        public async Task UpdateStubAsync(StubDto stub, string stubId)
        {
            using var response =
                await HttpClient.PutAsync($"/ph-api/stubs/{stubId}",
                    new StringContent(JsonConvert.SerializeObject(stub), Encoding.UTF8, JsonContentType));
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public Task UpdateStubAsync(StubBuilder stubBuilder, string stubId) =>
            UpdateStubAsync(stubBuilder.Build(), stubId);

        /// <inheritdoc />
        public async Task<IEnumerable<FullStubDto>> GetAllStubsAsync()
        {
            using var response = await HttpClient.GetAsync("/ph-api/stubs");
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
            using var response = await HttpClient.GetAsync("/ph-api/stubs/overview");
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
            using var response = await HttpClient.GetAsync($"/ph-api/stubs/{stubId}/requests");
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
            using var response = await HttpClient.GetAsync($"/ph-api/stubs/{stubId}");
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
            using var response = await HttpClient.DeleteAsync($"/ph-api/stubs/{stubId}");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAllStubsAsync()
        {
            using var response = await HttpClient.DeleteAsync("/ph-api/stubs");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetTenantNamesAsync()
        {
            using var response = await HttpClient.GetAsync("/ph-api/tenants");
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
            using var response = await HttpClient.GetAsync($"/ph-api/tenants/{tenant}/stubs");
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
            using var response = await HttpClient.DeleteAsync($"/ph-api/tenants/{tenant}/stubs");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubDto> stubs)
        {
            using var response = await HttpClient.PutAsync($"/ph-api/tenants/{tenant}/stubs",
                new StringContent(JsonConvert.SerializeObject(stubs), Encoding.UTF8, JsonContentType));
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public Task UpdateAllStubsByTenantAsync(string tenant, IEnumerable<StubBuilder> stubBuilders) =>
            UpdateAllStubsByTenantAsync(tenant, stubBuilders.Select(b => b.Build()));

        /// <inheritdoc />
        public async Task<UserDto> GetUserAsync(string username)
        {
            using var response = await HttpClient.GetAsync($"/ph-api/users/{username}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<UserDto>(content);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ScenarioStateDto>> GetAllScenarioStatesAsync()
        {
            using var response = await HttpClient.GetAsync("/ph-api/scenarios");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<ScenarioStateDto>>(content);
        }

        /// <inheritdoc />
        public async Task<ScenarioStateDto> GetScenarioStateAsync(string scenario)
        {
            using var response = await HttpClient.GetAsync($"/ph-api/scenarios/{scenario}");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<ScenarioStateDto>(content);
        }

        /// <inheritdoc />
        public async Task SetScenarioAsync(string scenario, ScenarioStateInputDto input)
        {
            using var response = await HttpClient.PutAsync($"/ph-api/scenarios/{scenario}",
                new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, JsonContentType));
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task DeleteScenarioAsync(string scenario)
        {
            using var response = await HttpClient.DeleteAsync($"/ph-api/scenarios/{scenario}");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAllScenariosAsync()
        {
            using var response = await HttpClient.DeleteAsync("/ph-api/scenarios");
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<FullStubDto>> CreateCurlStubsAsync(string input, bool doNotCreateStub)
        {
            using var response = await HttpClient.PostAsync(
                $"/ph-api/import/curl?doNotCreateStub={doNotCreateStub}",
                new StringContent(input,
                    Encoding.UTF8,
                    TextContentType));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttPlaceholderClientException(response.StatusCode, content);
            }

            return JsonConvert.DeserializeObject<IEnumerable<FullStubDto>>(content);
        }
    }
}
