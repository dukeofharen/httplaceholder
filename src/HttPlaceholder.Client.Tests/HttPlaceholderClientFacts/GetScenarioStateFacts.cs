using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetScenarioStateFacts : BaseClientTest
{
    private const string ScenarioResponse = @" {
        ""scenario"": ""scenario-1"",
        ""state"": ""new-state"",
        ""hitCount"": 10
    }";

    [TestMethod]
    public async Task GetScenarioState_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string scenario = "scenario-1";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/scenarios/{scenario}")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.GetScenarioStateAsync(scenario));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetScenarioState_ShouldReturnScenarioState()
    {
        // Arrange
        const string scenario = "scenario-1";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/scenarios/{scenario}")
            .Respond("application/json", ScenarioResponse)));

        // Act
        var result = await client.GetScenarioStateAsync(scenario);

        // Assert
        Assert.AreEqual("scenario-1", result.Scenario);
        Assert.AreEqual("new-state", result.State);
        Assert.AreEqual(10, result.HitCount);
    }
}
