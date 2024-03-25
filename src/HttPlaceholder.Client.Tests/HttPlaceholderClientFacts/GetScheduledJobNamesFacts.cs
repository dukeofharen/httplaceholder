using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class GetScheduledJobNamesFacts : BaseClientTest
{
    private const string GetScheduledJobsJson = """["CleanOldRequestsJob"]""";

    [TestMethod]
    public async Task ExecuteScheduledJobAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/scheduledJob")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() => client.GetScheduledJobNamesAsync());

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task ExecuteScheduledJobAsync_ShouldExecuteScheduledJob()
    {
        // Arrange
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Get, $"{BaseUrl}ph-api/scheduledJob")
            .Respond("application/json", GetScheduledJobsJson)));

        // Act
        var result = await client.GetScheduledJobNamesAsync();

        // Assert
        Assert.AreEqual(1, result.Length);
        Assert.AreEqual("CleanOldRequestsJob", result[0]);
    }
}
