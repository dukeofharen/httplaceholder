using System.Linq;
using System.Net;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetTenantNamesFacts : BaseClientTest
{
    private const string GetTenantNamesResponse = @"[
    ""01-get"",
    ""02-post""
]";

    [TestMethod]
    public async Task GetTenantNamesAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/tenants")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.GetTenantNamesAsync());

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetTenantNamesAsync_ShouldReturnTenantNames()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/tenants")
            .Respond("application/json", GetTenantNamesResponse)));

        // Act
        var result = (await client.GetTenantNamesAsync()).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("01-get", result[0]);
        Assert.AreEqual("02-post", result[1]);
    }
}
