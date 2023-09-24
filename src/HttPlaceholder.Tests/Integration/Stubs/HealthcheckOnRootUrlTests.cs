using System.Linq;
using System.Net;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class HealthcheckOnRootUrlTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize()
    {
        Settings.Stub.HealthcheckOnRootUrl = true;
        InitializeStubIntegrationTest("Resources/integration.yml");
    }

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_HealthcheckOnRootUrl_RootUrlCalled_ShouldNotExecuteStubs()
    {
        // Arrange
        var url = BaseAddress;

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK", content);

        var requests = await GetRequestsAsync();
        Assert.IsFalse(requests.Any());
    }

    [TestMethod]
    public async Task StubIntegration_HealthcheckOnRootUrl_NotRootUrlCalled_ShouldExecuteStubs()
    {
        // Arrange
        var url = $"{BaseAddress}some-url";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);

        var requests = await GetRequestsAsync();
        Assert.IsTrue(requests.Any());
    }
}
