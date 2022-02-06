using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiScheduledJobTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_ScheduledJobs_ScheduledJobNotFound_ShouldReturn404()
    {
        // Run a non-existent job.
        using var scheduledJobResponse = await Client.PostAsync($"{BaseAddress}ph-api/scheduledJob/NotExists", new StringContent(string.Empty));
        Assert.AreEqual(HttpStatusCode.NotFound, scheduledJobResponse.StatusCode);
    }
}
