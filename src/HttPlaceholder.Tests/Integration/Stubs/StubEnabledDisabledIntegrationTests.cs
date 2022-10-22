using System.Net;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubEnabledDisabledIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_StubIsEnabled_ShouldReturnContent()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}enabled";

        // act / assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("This stub is enabled.", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_StubIsDisabled_ShouldNotReturnContent()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}disabled";

        // act / assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreNotEqual("This stub is disabled.", content);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
