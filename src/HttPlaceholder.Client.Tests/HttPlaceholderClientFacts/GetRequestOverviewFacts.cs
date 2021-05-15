using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts
{
    [TestClass]
    public class GetRequestOverviewFacts : BaseClientTest
    {
        private const string RequestOverviewResponse = @"[
    {
        ""correlationId"": ""bec89e6a-9bee-4565-bccb-09f0a3363eee"",
        ""method"": ""POST"",
        ""url"": ""http://localhost:5000/post-xml-without-namespaces"",
        ""executingStubId"": ""xml-without-namespaces-specified"",
        ""stubTenant"": ""integration"",
        ""requestBeginTime"": ""2021-05-13T10:45:35.8461861Z"",
        ""requestEndTime"": ""2021-05-13T10:45:35.8635415Z""
    },
    {
        ""correlationId"": ""e7ad87fd-29db-4a57-84c8-063a27462810"",
        ""method"": ""POST"",
        ""url"": ""http://localhost:5000/post-xml-without-namespaces"",
        ""executingStubId"": ""xml-without-namespaces-specified"",
        ""stubTenant"": ""integration"",
        ""requestBeginTime"": ""2021-05-13T10:45:35.2001502Z"",
        ""requestEndTime"": ""2021-05-13T10:45:35.2213267Z""
    }
]";

        [TestMethod]
        public async Task GetRequestOverviewAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When($"{BaseUrl}ph-api/requests/overview")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                    client.GetRequestOverviewAsync());

            // Assert
            Assert.AreEqual($"Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task GetRequestOverviewAsync_ShouldReturnRequestOverview()
        {
            // Arrange
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When($"{BaseUrl}ph-api/requests/overview")
                .Respond("application/json", RequestOverviewResponse)));

            // Act
            var result = (await client.GetRequestOverviewAsync()).ToArray();

            // Assert
            Assert.AreEqual(2, result.Length);

            Assert.AreEqual("bec89e6a-9bee-4565-bccb-09f0a3363eee", result[0].CorrelationId);
            Assert.AreEqual("xml-without-namespaces-specified", result[0].ExecutingStubId);
        }
    }
}
