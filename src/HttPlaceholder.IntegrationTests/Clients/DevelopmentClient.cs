using System.Net;
using System.Text;
using HttPlaceholder.IntegrationTests.Config;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.IntegrationTests.Clients;

public class DevelopmentClient
{
    private readonly HttPlaceholderSettings _httPlaceholderSettings;
    private readonly HttpClient _httpClient;

    public DevelopmentClient(IOptions<HttPlaceholderSettings> options, HttpClient httpClient)
    {
        _httPlaceholderSettings = options.Value;
        _httpClient = httpClient;
    }

    public async Task SetDistributionKeyAsync(string key)
    {
        var url = $"{_httPlaceholderSettings.HttpUrl}/ph-development/set-distribution-key";
        var body = $$"""{"key": "{{key}}"}""";
        var response = await _httpClient.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }
}
