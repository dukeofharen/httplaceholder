using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class DeleteScenarioFacts : BaseClientTest
{
    [TestMethod]
    public async Task DeleteScenario_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string scenario = "scenario-1";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Delete, $"{BaseUrl}ph-api/scenarios/{scenario}")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.DeleteScenarioAsync(scenario));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetScenarioState_ShouldDeleteScenario()
    {
        // Arrange
        const string scenario = "scenario-1";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Delete, $"{BaseUrl}ph-api/scenarios/{scenario}")
            .Respond(HttpStatusCode.NoContent)));

        // Act / Assert
        await client.DeleteScenarioAsync(scenario);
    }
}
