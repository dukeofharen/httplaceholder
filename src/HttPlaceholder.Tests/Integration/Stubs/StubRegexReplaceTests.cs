namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubRegexReplaceTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_StringReplace_Regular()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}regex-replace";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("Lorem Bassie dolor sit Adriaan, Bassie adipiscing Adriaan.", content);
    }

    [TestMethod]
    public async Task StubIntegration_StringReplace_Dynamic()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}regex-replace-dynamic?q1=val1&q2=val2";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("Lorem val1 dolor sit val2, val1 adipiscing val2.", content);
    }
}
