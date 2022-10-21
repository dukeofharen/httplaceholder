using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class DeleteAllRequestsFacts : BaseClientTest
{
    [TestMethod]
    public async Task DeleteAllRequestsAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Delete, $"{BaseUrl}ph-api/requests")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.DeleteAllRequestsAsync());

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task DeleteAllRequestsAsync_ShouldDeleteRequests()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Delete, $"{BaseUrl}ph-api/requests")
            .Respond(HttpStatusCode.NoContent)));

        // Act / Assert
        await client.DeleteAllRequestsAsync();
    }
}
