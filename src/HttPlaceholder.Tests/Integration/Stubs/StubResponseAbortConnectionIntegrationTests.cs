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
        var exception = await Assert.ThrowsExceptionAsync<OperationCanceledException>(() => Client.GetAsync(url));

        // Assert
        Assert.IsTrue(exception.Message.Contains("The application aborted the request."));
    }
}
