using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiScheduledJobTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize()
    {
        Options.Value.Storage.CleanOldRequestsInBackgroundJob = true;
        InitializeRestApiIntegrationTest();
    }

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_ScheduledJobs_ScheduledJobNotFound_ShouldReturn404()
    {
        // Run a non-existent job.
        using var scheduledJobResponse = await Client.PostAsync($"{BaseAddress}ph-api/scheduledJob/NotExists", new StringContent(string.Empty));
        Assert.AreEqual(HttpStatusCode.NotFound, scheduledJobResponse.StatusCode);
    }

    [TestMethod]
    public async Task RestApiIntegration_ScheduledJobs_GetScheduledJobNames_HappyFlow()
    {
        // Perform the request.
        using var response = await Client.GetAsync($"{BaseAddress}ph-api/scheduledJob");
        var content = await response.Content.ReadAsStringAsync();
        var jobs = JsonConvert.DeserializeObject<string[]>(content);
        Assert.IsNotNull(jobs);

        // Check the jobs.
        Assert.AreEqual(1, jobs.Length);
        Assert.AreEqual("CleanOldRequestsJob", jobs[0]);
    }
}
