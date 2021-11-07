using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Dto.Scenarios;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts
{
    [TestClass]
    public class SetScenarioFacts : BaseClientTest
    {
        [TestMethod]
        public async Task SetScenario_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            const string scenario = "scenario-1";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Put, $"{BaseUrl}ph-api/scenarios/{scenario}")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                    client.SetScenarioAsync(scenario, new ScenarioStateInputDto()));

            // Assert
            Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task SetScenario_ShouldSetScenario()
        {
            // Arrange
            const string scenario = "scenario-1";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Put, $"{BaseUrl}ph-api/scenarios/{scenario}")
                .Respond(HttpStatusCode.NoContent)));

            // Act / Assert
            await client.SetScenarioAsync(scenario, new ScenarioStateInputDto());
        }
    }
}
