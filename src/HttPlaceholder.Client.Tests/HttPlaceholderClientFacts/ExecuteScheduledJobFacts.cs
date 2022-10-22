using System.Net;
using System.Net.Http;
using HttPlaceholder.Client.Exceptions;
using HttPlaceholder.Client.Implementations;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Client.Tests.HttPlaceholderClientFacts;

[TestClass]
public class ExecuteScheduledJobFacts : BaseClientTest
{
    private const string ExecuteJobJson = @"{
    ""message"": ""OK""
}";

    [TestMethod]
    public async Task ExecuteScheduledJobAsync_ExceptionInRequest_ShouldThrowHttPlaceholderClientException()
    {
        // Arrange
        const string jobName = "CleanOldRequestsJobs";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/scheduledJob/{jobName}")
            .Respond(HttpStatusCode.BadRequest, "text/plain", "Error occurred!")));

        // Act
        var exception =
            await Assert.ThrowsExceptionAsync<HttPlaceholderClientException>(() =>
                client.ExecuteScheduledJobAsync(jobName));

        // Assert
        Assert.AreEqual("Status code '400' returned by HttPlaceholder with message 'Error occurred!'",
            exception.Message);
    }

    [TestMethod]
    public async Task ExecuteScheduledJobAsync_ShouldExecuteScheduledJob()
    {
        // Arrange
        const string jobName = "CleanOldRequestsJobs";
        var client = new HttPlaceholderClient(CreateHttpClient(mock => mock
            .When(HttpMethod.Post, $"{BaseUrl}ph-api/scheduledJob/{jobName}")
            .Respond("application/json", ExecuteJobJson)));

        // Act
        var result = await client.ExecuteScheduledJobAsync(jobName);

        // Assert
        Assert.AreEqual("OK", result.Message);
    }
}
