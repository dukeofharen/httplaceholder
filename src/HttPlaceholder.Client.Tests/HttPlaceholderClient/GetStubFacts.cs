using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClient
{
    [TestClass]
    public class GetStubFacts : BaseClientTest
    {
        private const string GetStubResponse = @"{
    ""stub"": {
        ""id"": ""fallback"",
        ""response"": {
            ""text"": ""OK FALLBACK""
        },
        ""priority"": -1,
        ""tenant"": ""integration"",
        ""enabled"": true
    },
    ""metadata"": {
        ""readOnly"": false
    }
}";

        [TestMethod]
        public async Task GetStubAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            var stubId = "fallback";
            var client = new Implementations.HttPlaceholderClient(CreateHttpClient(mock => mock
                .When($"{BaseUrl}ph-api/stubs/{stubId}")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                    client.GetStubAsync(stubId));

            // Assert
            Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task GetStubAsync_ShouldReturnStub()
        {
            // Arrange
            var stubId = "fallback";
            var client = new Implementations.HttPlaceholderClient(CreateHttpClient(mock => mock
                .When($"{BaseUrl}ph-api/stubs/{stubId}")
                .Respond("application/json", GetStubResponse)));

            // Act
            var result = await client.GetStubAsync(stubId);

            // Assert
            Assert.IsNotNull(result.Metadata);
            Assert.IsNotNull(result.Stub);
            Assert.AreEqual(stubId, result.Stub.Id);
            Assert.AreEqual("OK FALLBACK", result.Stub.Response.Text);
        }
    }
}
