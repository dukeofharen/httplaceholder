namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubStringReplaceTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_StringReplace_Regular()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}string-replace";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK STRING REPLACE", content);
    }

    [TestMethod]
    public async Task StubIntegration_StringReplace_Dynamic()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}string-replace-dynamic?q1=val1&q2=val2";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("val1 val2", content);
    }
}
