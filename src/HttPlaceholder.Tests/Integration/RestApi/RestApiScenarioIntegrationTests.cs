using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Dto.v1.Scenarios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi
{
    [TestClass]
    public class RestApiScenarioIntegrationTests : RestApiIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeRestApiIntegrationTest();

        [TestCleanup]
        public void Cleanup() => CleanupRestApiIntegrationTest();

        [TestMethod]
        public async Task RestApiIntegration_Scenario_GetScenario_NotFound()
        {
            // Arrange
            const string scenario = "scenario-1";

            // Act
            using var response = await GetScenarioAsync(scenario);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Scenario_SetAndGetScenarioAndGetAllScenarios_Found()
        {
            // Arrange
            const string scenario = "scenario-1";
            var input = new ScenarioStateInputDto {State = "new-state", HitCount = 20};
            using var setScenarioResponse = await SetScenarioAsync(scenario, input);
            setScenarioResponse.EnsureSuccessStatusCode();

            // Act: get scenario
            using var response = await GetScenarioAsync(scenario);

            // Assert: get scenario
            response.EnsureSuccessStatusCode();
            var scenarioState =
                JsonConvert.DeserializeObject<ScenarioStateDto>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(scenario, scenarioState.Scenario);
            Assert.AreEqual(input.State, scenarioState.State);
            Assert.AreEqual(input.HitCount, scenarioState.HitCount);

            // Act: get all scenarios
            using var allScenariosResponse = await GetAllScenariosAsync();

            // Assert: get all scenarios
            allScenariosResponse.EnsureSuccessStatusCode();
            var scenarios =
                JsonConvert.DeserializeObject<ScenarioStateDto[]>(
                    await allScenariosResponse.Content.ReadAsStringAsync());
            Assert.IsNotNull(scenarios);
            Assert.AreEqual(1, scenarios.Length);
            Assert.AreEqual(scenario, scenarios[0].Scenario);
            Assert.AreEqual(input.State, scenarios[0].State);
            Assert.AreEqual(input.HitCount, scenarios[0].HitCount);
        }

        [TestMethod]
        public async Task RestApiIntegration_Scenario_DeleteScenario_NotFound()
        {
            // Arrange
            const string scenario = "scenario-1";

            // Act
            using var response = await DeleteScenarioAsync(scenario);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Scenario_SetDeleteAndGetScenario_Found()
        {
            // Arrange
            const string scenario = "scenario-1";
            using var setScenarioResponse =
                await SetScenarioAsync(scenario, new ScenarioStateInputDto {State = "new-state", HitCount = 20});
            setScenarioResponse.EnsureSuccessStatusCode();

            // Act
            using var response = await DeleteScenarioAsync(scenario);

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            using var getScenarioResponse = await GetScenarioAsync(scenario);
            Assert.AreEqual(HttpStatusCode.NotFound, getScenarioResponse.StatusCode);
        }

        [TestMethod]
        public async Task RestApiIntegration_Scenario_SetDeleteAllAndGetScenario_Found()
        {
            // Arrange
            const string scenario = "scenario-1";
            using var setScenarioResponse =
                await SetScenarioAsync(scenario, new ScenarioStateInputDto {State = "new-state", HitCount = 20});
            setScenarioResponse.EnsureSuccessStatusCode();

            // Act
            using var response = await DeleteAllScenariosAsync();

            // Assert
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
            using var getScenarioResponse = await GetScenarioAsync(scenario);
            Assert.AreEqual(HttpStatusCode.NotFound, getScenarioResponse.StatusCode);
        }

        private async Task<HttpResponseMessage> GetScenarioAsync(string scenario) =>
            await Client.GetAsync($"{BaseAddress}ph-api/scenarios/{scenario}");

        private async Task<HttpResponseMessage> GetAllScenariosAsync() =>
            await Client.GetAsync($"{BaseAddress}ph-api/scenarios/");

        private async Task<HttpResponseMessage> SetScenarioAsync(string scenario, ScenarioStateInputDto input)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"{BaseAddress}ph-api/scenarios/{scenario}"),
                Content = new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json")
            };
            return await Client.SendAsync(request);
        }

        private async Task<HttpResponseMessage> DeleteScenarioAsync(string scenario) =>
            await Client.DeleteAsync($"{BaseAddress}ph-api/scenarios/{scenario}");

        private async Task<HttpResponseMessage> DeleteAllScenariosAsync() =>
            await Client.DeleteAsync($"{BaseAddress}ph-api/scenarios");
    }
}
