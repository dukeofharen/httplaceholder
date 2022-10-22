using System.Net;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetResponseFacts : BaseClientTest
{
    private const string ResponseResponse = @"{
    ""statusCode"": 200,
    ""body"": ""OGU0Y2I2OGItMmU2Yi00NDBjLWEwYTAtNzRiNTcyMzIwZGUyCmVkMTk2ZGY2LWRmMzItNDhjMC1iMzE5LWM2OThmNGY4NjhiYg=="",
    ""bodyIsBinary"": false,
    ""headers"": {
        ""Content-Type"": ""text/plain""
    }
}";

    [TestMethod]
    public async Task GetResponseAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string correlationId = "bec89e6a-9bee-4565-bccb-09f0a3363eee";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/requests/{correlationId}/response")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.GetResponseAsync(correlationId));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetResponseAsync_ShouldReturnResponse()
    {
        // Arrange
        const string correlationId = "bec89e6a-9bee-4565-bccb-09f0a3363eee";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/requests/{correlationId}/response")
            .Respond("application/json", ResponseResponse)));

        // Act
        var result = await client.GetResponseAsync(correlationId);

        // Assert
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(73, result.Body.Length);
        Assert.IsFalse(result.BodyIsBinary);
        Assert.AreEqual("text/plain", result.Headers["Content-Type"]);
    }
}
