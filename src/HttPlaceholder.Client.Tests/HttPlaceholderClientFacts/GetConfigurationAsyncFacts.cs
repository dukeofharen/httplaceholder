using System.Linq;
using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetConfigurationAsyncFacts : BaseClientTest
{
    private const string GetConfigJson = """
                                         [
                                             {
                                                 "key": "httpsport",
                                                 "path": "Web:HttpsPort",
                                                 "description": "the port HttPlaceholder should run under when HTTPS is enabled. Listen on multiple ports by separating ports with comma.",
                                                 "configKeyType": "Web",
                                                 "value": "5050"
                                             },
                                             {
                                                 "key": "oldrequestsqueuelength",
                                                 "path": "Storage:OldRequestsQueueLength",
                                                 "description": "the maximum number of HTTP requests that the in memory stub source (used for the REST API) should store before truncating old records. Default: 40.",
                                                 "configKeyType": "Storage",
                                                 "value": "40"
                                             }
                                         ]
                                         """;

    [TestMethod]
    public async Task GetConfigurationAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/configuration")
            .Respond(HttpStatusCode.NotFound, "text/plain", "not found")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.GetConfigurationAsync());

        // Assert
        Assert.AreEqual("Status code '404' returned by HttPlaceholder with message 'not found'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetConfigurationAsync_ShouldReturnConfig()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/configuration")
            .Respond("application/json", GetConfigJson)));

        // Act
        var result = (await client.GetConfigurationAsync()).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("Web:HttpsPort", result[0].Path);
    }
}
