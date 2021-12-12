using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetStubOverviewFacts : BaseClientTest
{
    private const string GetStubOverviewResponse = @"[
    {
        ""stub"": {
            ""id"": ""post-with-json-object-checker"",
            ""tenant"": ""integration"",
            ""enabled"": true
        },
        ""metadata"": {
            ""readOnly"": false
        }
    },
    {
        ""stub"": {
            ""id"": ""temporary-redirect"",
            ""tenant"": ""integration"",
            ""enabled"": true
        },
        ""metadata"": {
            ""readOnly"": false
        }
    }
]";

    [TestMethod]
    public async Task GetStubOverviewAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/overview")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.GetStubOverviewAsync());

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task GetStubOverviewAsync_ShouldReturnStubOverview()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When($"{BaseUrl}ph-api/stubs/overview")
            .Respond("application/json", GetStubOverviewResponse)));

        // Act
        var result = (await client.GetStubOverviewAsync()).ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);

        Assert.AreEqual("post-with-json-object-checker", result[0].Stub.Id);
        Assert.AreEqual("temporary-redirect", result[1].Stub.Id);
    }
}