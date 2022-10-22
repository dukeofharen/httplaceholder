using System.Linq;
using System.Net;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetRequestFacts : BaseClientTest
{
    private const string RequestResponse = @"{
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
        }
    ],
    ""stubResponseWriterResults"": [
        {
            ""responseWriterName"": ""StatusCodeResponseWriter"",
            ""executed"": true
        }
    ],
    ""executingStubId"": ""xml-without-namespaces-specified"",
    ""stubTenant"": ""integration"",
    ""requestBeginTime"": ""2021-05-13T10:45:35.8461861Z"",
    ""requestEndTime"": ""2021-05-13T10:45:35.8635415Z""
}";

    [TestMethod]
    public async Task GetRequestAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string correlationId = "bec89e6a-9bee-4565-bccb-09f0a3363eee";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/requests/{correlationId}")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(
                () => client.GetRequestAsync(correlationId));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetRequestAsync_ShouldReturnRequest()
    {
        // Arrange
        const string correlationId = "bec89e6a-9bee-4565-bccb-09f0a3363eee";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/requests/{correlationId}")
            .Respond("application/json", RequestResponse)));

        // Act
        var result = await client.GetRequestAsync(correlationId);

        // Assert
        Assert.AreEqual(correlationId, result.CorrelationId);
        Assert.AreEqual("POST", result.RequestParameters.Method);
        Assert.AreEqual(8, result.RequestParameters.Headers.Count);
        Assert.AreEqual("PostmanRuntime/7.26.8", result.RequestParameters.Headers["User-Agent"]);
        Assert.AreEqual(1, result.StubExecutionResults.Count);
        Assert.AreEqual("xml-without-namespaces-specified", result.ExecutingStubId);

        var stubExecutionResult = result.StubExecutionResults[0];
        Assert.AreEqual("post-with-json-object-checker", stubExecutionResult.StubId);
        Assert.AreEqual("MethodConditionChecker", stubExecutionResult.Conditions.ElementAt(0).CheckerName);

        Assert.AreEqual(1, result.StubResponseWriterResults.Count);
        Assert.AreEqual("StatusCodeResponseWriter", result.StubResponseWriterResults[0].ResponseWriterName);
    }
}
