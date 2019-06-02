using System;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Client.Configuration;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Client
{
    public class HttPlaceholderClientFactory : IHttPlaceholderClientFactory
    {
        private readonly HttpClient _httpClient;
        private readonly HttPlaceholderClientSettings _settings;

        public HttPlaceholderClientFactory(
            HttpClient httpClient,
            IOptions<HttPlaceholderClientSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;

            if (!string.IsNullOrWhiteSpace(_settings.Username) && !string.IsNullOrWhiteSpace(_settings.Password))
            {
                string basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.Username}:{_settings.Password}"));
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {basic}");
            }
        }

        public IMetadataClient MetadataClient => new MetadataClient(_httpClient)
        {
            BaseUrl = _settings.BaseUrl
        };

        public IRequestClient RequestClient => new RequestClient(_httpClient)
        {
            BaseUrl = _settings.BaseUrl
        };

        public IStubClient StubClient => new StubClient(_httpClient)
        {
            BaseUrl = _settings.BaseUrl
        };

        public ITenantClient TenantClient => new TenantClient(_httpClient)
        {
            BaseUrl = _settings.BaseUrl
        };

        public IUserClient UserClient => new UserClient(_httpClient)
        {
            BaseUrl = _settings.BaseUrl
        };
    }
}
