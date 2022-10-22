using System.Linq;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubContentTypeIntegrationTest : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_ContentType_ShouldReturnCorrectContentType()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}content-type.csv";

        // Act
        using var response = await Client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsFalse(string.IsNullOrWhiteSpace(content));
        Assert.AreEqual("text/csv", response.Content.Headers.Single(h => h.Key == Constants.ContentType).Value.Single());
    }
}
