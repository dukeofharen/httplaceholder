namespace HttPlaceholder.Tests.Integration.Stubs.Scenarios;

[TestClass]
public class HitCounterTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/scenarios.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_HitCounter_MinMax()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}scenario-min-max-hits";
        var expectedResponses = new[]
        {
            "Ok, hits are increased", "Ok, hit goal is reached :)", "Ok, hit goal is reached :)",
            "Ok, hits are increased"
        };
        foreach (var expectedResponse in expectedResponses)
        {
            using var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(expectedResponse, content);
        }
    }

    [TestMethod]
    public async Task StubIntegration_HitCounter_Exact()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}scenario-exact-hits";
        var expectedResponses = new[]
        {
            "Ok, hits are increased", "Ok, exact hit goal is reached :)", "Ok, hits are increased",
            "Ok, hits are increased"
        };
        foreach (var expectedResponse in expectedResponses)
        {
            using var response = await Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(expectedResponse, content);
        }
    }
}
