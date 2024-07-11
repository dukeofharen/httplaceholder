using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubJsonPathReplaceTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_StringReplace_Regular()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}jsonpath-replace";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var jObject = JObject.Parse(content);
        Assert.AreEqual("Adriaan", jObject["name"]);
        Assert.AreEqual("Groningen", jObject["city"]);
    }

    [TestMethod]
    public async Task StubIntegration_StringReplace_Dynamic()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}jsonpath-replace-dynamic?q1=Pipo&q2=Assen";

        // Act
        var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var jObject = JObject.Parse(content);
        Assert.AreEqual("Pipo", jObject["name"]);
        Assert.AreEqual("Assen", jObject["city"]);
    }
}
