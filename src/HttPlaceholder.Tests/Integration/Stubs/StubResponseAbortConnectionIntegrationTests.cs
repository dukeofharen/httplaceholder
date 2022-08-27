using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubResponseAbortConnectionIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_AbortConnection()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}response-abort-connection";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        // Not really possible to test an aborted connection in an integration test, so let's test it like this.
        Assert.IsTrue(Requests.Any(r => r.ExecutingStubId == "response-abort-connection"));
    }
}
