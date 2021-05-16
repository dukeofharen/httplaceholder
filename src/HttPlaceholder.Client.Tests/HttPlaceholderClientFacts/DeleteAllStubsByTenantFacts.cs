using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts
{
    [TestClass]
    public class DeleteAllStubsByTenantFacts : BaseClientTest
    {
        [TestMethod]
        public async Task DeleteAllStubAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
        {
            // Arrange
            const string tenant = "01-get";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Delete, $"{BaseUrl}ph-api/tenants/{tenant}/stubs")
                .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

            // Act
            var exception =
                await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                    client.DeleteAllStubsByTenantAsync(tenant));

            // Assert
            Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
                exception.Message);
        }

        [TestMethod]
        public async Task DeleteAllStubAsync_ShouldDeleteStubs()
        {
            // Arrange
            const string tenant = "01-get";
            var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
                .When(HttpMethod.Delete, $"{BaseUrl}ph-api/tenants/{tenant}/stubs")
                .Respond(HttpStatusCode.NoContent)));

            // Act / Assert
            await client.DeleteAllStubsByTenantAsync(tenant);
        }
    }
}
