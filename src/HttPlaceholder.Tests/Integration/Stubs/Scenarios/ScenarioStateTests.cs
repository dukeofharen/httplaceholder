using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs.Scenarios
{
    [TestClass]
    public class ScenarioStateTests : StubIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeStubIntegrationTest("scenarios.yml");

        [TestCleanup]
        public void Cleanup() => CleanupIntegrationTest();

        [TestMethod]
        public async Task StubIntegration_ScenarioStates()
        {
            // Arrange
            var url = $"{TestServer.BaseAddress}scenario-state";
            var expectedResponses = new[]
            {
                "Ok, scenario is now set to state-1",
                "Ok, scenario is now set to state-2",
                "Ok, scenario is set to its original state",
                "Ok, scenario is now set to state-1"
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
}
