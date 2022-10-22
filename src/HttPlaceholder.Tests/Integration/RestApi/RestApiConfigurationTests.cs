using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Dto.v1.Configuration;
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
}
