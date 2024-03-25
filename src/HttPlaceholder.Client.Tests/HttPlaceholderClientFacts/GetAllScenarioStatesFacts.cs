using System.Linq;
using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetAllScenarioStatesFacts : BaseClientTest
{
    private const string ScenariosResponse = """
                                             [
                                                 {
                                                     "scenario": "scenario-1",
                                                     "state": "new-state",
                                                     "hitCount": 10
                                                 },
                                                 {
                                                     "scenario": "scenario-2",
                                                     "state": "new-state",
                                                     "hitCount": 10
                                                 }
                                             ]
                                             """;

    [TestMethod]
    public async Task GetAllScenarioStates_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/scenarios")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.GetAllScenarioStatesAsync());

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetAllScenarioStates_ShouldReturnAllScenarioStates()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/scenarios")
            .Respond("application/json", ScenariosResponse)));

        // Act
        var result = (await client.GetAllScenarioStatesAsync()).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("scenario-1", result[0].Scenario);
        Assert.AreEqual("new-state", result[0].State);
        Assert.AreEqual(10, result[0].HitCount);
    }
}
