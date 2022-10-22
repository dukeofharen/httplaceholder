using System.Linq;
using System.Net;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetRequestsByStubIdFacts : BaseClientTest
{
    private const string GetRequestsResponse = @"[
    {
        ""correlationId"": ""95890e55-0be2-4c40-9046-7c7b291693ce"",
        ""requestParameters"": {
            ""method"": ""GET"",
            ""url"": ""http://localhost:5000/test.html"",
            ""body"": """",
            ""headers"": {
                ""Connection"": ""keep-alive"",
                ""Accept"": ""text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"",
                ""Accept-Encoding"": ""gzip, deflate"",
                ""Accept-Language"": ""en-US,en;q=0.5"",
                ""Host"": ""localhost:5000"",
                ""User-Agent"": ""Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:88.0) Gecko/20100101 Firefox/88.0"",
                ""Upgrade-Insecure-Requests"": ""1""
            },
            ""clientIp"": ""127.0.0.1""
        },
        ""stubExecutionResults"": [
            {
                ""stubId"": ""post-with-json-object-checker"",
                ""passed"": false,
                ""conditions"": [
                    {
                        ""checkerName"": ""MethodConditionChecker"",
                        ""conditionValidation"": ""Invalid"",
                        ""log"": ""Condition 'POST' did not pass for request.""
                    }
                ]
            }
        ],
        ""stubResponseWriterResults"": [
            {
                ""responseWriterName"": ""StatusCodeResponseWriter"",
                ""executed"": true
            }
        ],
        ""executingStubId"": ""fallback"",
        ""stubTenant"": ""integration"",
        ""requestBeginTime"": ""2021-05-15T17:10:48.5785308Z"",
        ""requestEndTime"": ""2021-05-15T17:10:48.6514708Z""
    }
]";

    [TestMethod]
    public async Task GetRequestsByStubIdAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string stubId = "fallback";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.GetRequestsByStubIdAsync(stubId));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetRequestsByStubIdAsync_ShouldReturnRequests()
    {
        // Arrange
        const string stubId = "fallback";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/{stubId}/requests")
            .Respond("application/json", GetRequestsResponse)));

        // Act
        var result = (await client.GetRequestsByStubIdAsync(stubId)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var request = result.Single();
        Assert.AreEqual("95890e55-0be2-4c40-9046-7c7b291693ce", request.CorrelationId);
    }
}
