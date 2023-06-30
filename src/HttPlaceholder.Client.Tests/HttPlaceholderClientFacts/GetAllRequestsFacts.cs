using System.Linq;
using System.Net;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetAllRequestsFacts : BaseClientTest
{
    private const string AllRequestsResponse = @"[
    {
        ""correlationId"": ""bec89e6a-9bee-4565-bccb-09f0a3363eee"",
        ""requestParameters"": {
            ""method"": ""POST"",
            ""url"": ""http://localhost:5000/post-xml-without-namespaces"",
            ""body"": ""<?xml version=\""1.0\""?>\n<soap:Envelope xmlns:soap=\""http://www.w3.org/2003/05/soap-envelope\"" \n    xmlns:m=\""http://www.example.org/stock/GetUser\"">\n    <soap:Header></soap:Header>\n    <soap:Body>\n        <m:GetUser>\n            <m:Username>user1</m:Username>\n        </m:GetUser>\n    </soap:Body>\n</soap:Envelope>"",
            ""headers"": {
                ""Connection"": ""keep-alive"",
                ""Content-Type"": ""application/soap+xml; charset=utf-8"",
                ""Accept"": ""*/*"",
                ""Accept-Encoding"": ""gzip, deflate, br"",
                ""Host"": ""localhost:5000"",
                ""User-Agent"": ""PostmanRuntime/7.26.8"",
                ""Content-Length"": ""308"",
                ""Postman-Token"": ""4fdfadc4-3107-45ad-b31a-c4ed0300ea0f""
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
                        ""conditionValidation"": ""Valid""
                    },
                    {
                        ""checkerName"": ""PathConditionChecker"",
                        ""conditionValidation"": ""Invalid"",
                        ""log"": ""Condition '/post-with-json-object-checker' did not pass for request.""
                    }
                ]
            },
            {
                ""stubId"": ""temporary-redirect"",
                ""passed"": false,
                ""conditions"": [
                    {
                        ""checkerName"": ""MethodConditionChecker"",
                        ""conditionValidation"": ""Invalid"",
                        ""log"": ""Condition 'GET' did not pass for request.""
                    }
                ]
            }
        ],
        ""stubResponseWriterResults"": [
            {
                ""responseWriterName"": ""StatusCodeResponseWriter"",
                ""executed"": true
            },
            {
                ""responseWriterName"": ""TextResponseWriter"",
                ""executed"": true
            }
        ],
        ""executingStubId"": ""xml-without-namespaces-specified"",
        ""stubTenant"": ""integration"",
        ""requestBeginTime"": ""2021-05-13T10:45:35.8461861Z"",
        ""requestEndTime"": ""2021-05-13T10:45:35.8635415Z""
    }
]";


    [TestMethod]
    public async Task GetAllRequestsAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/requests")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.GetAllRequestsAsync());

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetAllRequestsAsync_ShouldReturnAllRequests()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/requests")
            .Respond("application/json", AllRequestsResponse)));

        // Act
        var result = (await client.GetAllRequestsAsync()).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var request = result.Single();
        Assert.AreEqual("bec89e6a-9bee-4565-bccb-09f0a3363eee", request.CorrelationId);
        Assert.AreEqual("POST", request.RequestParameters.Method);
        Assert.AreEqual(8, request.RequestParameters.Headers.Count);
        Assert.AreEqual("PostmanRuntime/7.26.8", request.RequestParameters.Headers["User-Agent"]);
        Assert.AreEqual(2, request.StubExecutionResults.Count);
        Assert.AreEqual("xml-without-namespaces-specified", request.ExecutingStubId);

        var stubExecutionResult = request.StubExecutionResults[0];
        Assert.AreEqual("post-with-json-object-checker", stubExecutionResult.StubId);
        Assert.AreEqual("MethodConditionChecker", stubExecutionResult.Conditions.ElementAt(0).CheckerName);

        Assert.AreEqual(2, request.StubResponseWriterResults.Count);
        Assert.AreEqual("StatusCodeResponseWriter", request.StubResponseWriterResults[0].ResponseWriterName);
    }

    [TestMethod]
    public async Task GetAllRequestsAsync_Paging_ShouldReturnAllRequests()
    {
        // Arrange
        const string fromIdentifier = "abc123";
        const int itemsPerPage = 3;
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/requests")
            .WithHeaders("x-from-identifier", fromIdentifier)
            .WithHeaders("x-items-per-page", itemsPerPage.ToString())
            .Respond("application/json", AllRequestsResponse)));

        // Act
        var result = (await client.GetAllRequestsAsync(fromIdentifier, itemsPerPage)).ToArray();

        // Assert
        Assert.AreEqual(1, result.Length);

        var request = result.Single();
        Assert.AreEqual("bec89e6a-9bee-4565-bccb-09f0a3363eee", request.CorrelationId);
        Assert.AreEqual("POST", request.RequestParameters.Method);
        Assert.AreEqual(8, request.RequestParameters.Headers.Count);
        Assert.AreEqual("PostmanRuntime/7.26.8", request.RequestParameters.Headers["User-Agent"]);
        Assert.AreEqual(2, request.StubExecutionResults.Count);
        Assert.AreEqual("xml-without-namespaces-specified", request.ExecutingStubId);

        var stubExecutionResult = request.StubExecutionResults[0];
        Assert.AreEqual("post-with-json-object-checker", stubExecutionResult.StubId);
        Assert.AreEqual("MethodConditionChecker", stubExecutionResult.Conditions.ElementAt(0).CheckerName);

        Assert.AreEqual(2, request.StubResponseWriterResults.Count);
        Assert.AreEqual("StatusCodeResponseWriter", request.StubResponseWriterResults[0].ResponseWriterName);
    }
}
