using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class DeleteAllStubsByTenantAsyncFacts : BaseClientTest
{
    [TestMethod]
    public async Task DeleteAllStubsByTenantAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string tenant = "tenantName";
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
    public async Task DeleteAllStubsByTenantAsync_ShouldDeleteStub()
    {
        // Arrange
        const string tenant = "tenantName";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Delete, $"{BaseUrl}ph-api/tenants/{tenant}/stubs")
            .Respond(HttpStatusCode.NoContent)));

        // Act / Assert
        await client.DeleteAllStubsByTenantAsync(tenant);
    }
}
