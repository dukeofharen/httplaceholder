using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts
{
    [TestClass]
    public class GetUserFacts : BaseClientTest
    {
        private const string GetUserResponse = @"{
    ""username"": ""user""
}";

        [TestMethod]
        public async Task GetRequestAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            var username = "user";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When($"{BaseUrl}ph-api/users/{username}")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.GetUserAsync(username));

            // Assert
            Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task GetRequestAsync_ShouldReturnUser()
        {
            // Arrange
            var username = "user";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When($"{BaseUrl}ph-api/users/{username}")
                .Respond("application/json", GetUserResponse)));

            // Act
            var result = await client.GetUserAsync(username);

            // Assert
            Assert.AreEqual(username, result.Username);
        }
    }
}
