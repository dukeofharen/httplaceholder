using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using HttPlaceholder.Web.Shared.Dto.v1.Configuration;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiConfigurationTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_GetConfiguration()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}ph-api/configuration";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var config = JsonConvert.DeserializeObject<IEnumerable<ConfigurationDto>>(content);
        Assert.IsNotNull(config);
        Assert.AreEqual(2, config.Count());
    }

    [TestMethod]
    public async Task RestApiIntegration_UpdateConfiguration()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}ph-api/configuration";
        var request = new UpdateConfigurationValueInputDto {ConfigurationKey = "storeResponses", NewValue = "true"};

        // Act
        using var response = await Client.PatchAsync(url,
            new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, MimeTypes.JsonMime));

        // Assert
        response.EnsureSuccessStatusCode();

        // Act
        using var getConfigResponse = await Client.GetAsync(url);

        // Assert
        getConfigResponse.EnsureSuccessStatusCode();
        var content = await getConfigResponse.Content.ReadAsStringAsync();
        var config = JsonConvert.DeserializeObject<ConfigurationDto[]>(content);
        Assert.IsNotNull(config);
        Assert.AreEqual(3, config.Length);
        var storeResponsesConfig = config.Single(c => c.Key == "storeResponses");
        Assert.AreEqual("true", storeResponsesConfig.Value);
    }
}
