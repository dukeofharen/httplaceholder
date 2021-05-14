using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests
{
    [TestClass]
    public class MetadataTests
    {
        private const string MetadataResponse = @"{
    ""version"": ""2019.8.24.1234"",
    ""variableHandlers"": [
        {
            ""name"": ""client_ip"",
            ""fullName"": ""Client IP variable handler"",
            ""example"": ""((client_ip))""
        },
        {
            ""name"": ""display_url"",
            ""fullName"": ""Display URL variable handler"",
            ""example"": ""((display_url))""
        },
        {
            ""name"": ""query_encoded"",
            ""fullName"": ""URL encoded query string variable handler"",
            ""example"": ""((query_encoded:query_string_key))""
        }
    ]
}";
        private const string BaseUrl = "http://localhost:5000/";
        private readonly MockHttpMessageHandler _mockHttp = new();

        [TestCleanup]
        public void Cleanup() => _mockHttp.VerifyNoOutstandingExpectation();

        [TestMethod]
        public async Task GetMetadataAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            _mockHttp
                .When($"{BaseUrl}ph-api/metadata")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!");
            var client = new HttPlaceholderClient(_mockHttp.ToHttpClient());

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.GetMetadataAsync());

            // Assert
            Assert.AreEqual($"Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task GetMetadataAsync_ShouldReturnMetadata()
        {
            // Arrange
            _mockHttp
                .When($"{BaseUrl}ph-api/metadata")
                .Respond("application/json", MetadataResponse);
            var client = new HttPlaceholderClient(_mockHttp.ToHttpClient());

            // Act
            var result = await client.GetMetadataAsync();

            // Assert
            Assert.AreEqual("2019.8.24.1234", result.Version);

            var variableHandlers = result.VariableHandlers.ToArray();
            Assert.AreEqual(3, variableHandlers.Length);
            Assert.AreEqual("client_ip", variableHandlers[0].Name);
            Assert.AreEqual("Client IP variable handler", variableHandlers[0].FullName);
            Assert.AreEqual("((client_ip))", variableHandlers[0].Example);
        }
    }
}
