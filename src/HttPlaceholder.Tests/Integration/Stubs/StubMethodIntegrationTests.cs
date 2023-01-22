using System.Net;
using System.Net.Http;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubMethodIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [DataTestMethod]
    [DataRow("GET", true)]
    [DataRow("get", true)]
    [DataRow("POST", true)]
    [DataRow("post", true)]
    [DataRow("PUT", false)]
    [DataRow("put", false)]
    public async Task StubIntegration_Method_MultipleMethods(string method, bool shouldPass)
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}multiple-methods";
        var request = new HttpRequestMessage {Method = new HttpMethod(method), RequestUri = new Uri(url)};

        // Act
        using var response = await Client.SendAsync(request);

        // Assert
        if (shouldPass)
        {
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("OK MULTIPLE METHODS", content);
        }
        else
        {
            Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
        }
    }
}
