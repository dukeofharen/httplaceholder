﻿using System.Collections.Generic;
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

        public Task<IEnumerable<RequestResultDto>> GetAllRequestsAsync() => throw new System.NotImplementedException();
    }
}
