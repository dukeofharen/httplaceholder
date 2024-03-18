using System.Linq;
using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetAllStubsFacts : BaseClientTest
{
    private const string GetAllStubsResponse = """
                                               [
                                                   {
                                                       "stub": {
                                                           "id": "temporary-redirect",
                                                           "conditions": {
                                                               "method": "GET",
                                                               "url": {
                                                                   "path": "^/temp-redirect$"
                                                               }
                                                           },
                                                           "response": {
                                                               "temporaryRedirect": "http://localhost:5000/temp-redirect-location"
                                                           },
                                                           "priority": 0,
                                                           "tenant": "integration",
                                                           "enabled": true
                                                       },
                                                       "metadata": {
                                                           "readOnly": false
                                                       }
                                                   },
                                                   {
                                                       "stub": {
                                                           "id": "dynamic-mode-form-post",
                                                           "conditions": {
                                                               "method": "POST",
                                                               "url": {
                                                                   "path": "/dynamic-mode-form-post"
                                                               }
                                                           },
                                                           "response": {
                                                               "enableDynamicMode": true,
                                                               "text": "Form post: ((form_post:var2))",
                                                               "headers": {
                                                                   "X-FormPost": "Header form post: ((form_post:var1))"
                                                               }
                                                           },
                                                           "priority": 0,
                                                           "tenant": "integration",
                                                           "enabled": true
                                                       },
                                                       "metadata": {
                                                           "readOnly": false
                                                       }
                                                   }
                                               ]
                                               """;

    [TestMethod]
    public async Task GetAllStubsAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/stubs")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.GetAllStubsAsync());

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetAllStubsAsync_ShouldReturnAllStubs()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/stubs")
            .Respond("application/json", GetAllStubsResponse)));

        // Act
        var result = (await client.GetAllStubsAsync()).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("temporary-redirect", result[0].Stub.Id);
        Assert.AreEqual("dynamic-mode-form-post", result[1].Stub.Id);
    }
}
