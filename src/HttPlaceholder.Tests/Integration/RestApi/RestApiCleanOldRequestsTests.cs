using System.Net.Http;
using HttPlaceholder.Dto.v1.Requests;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiCleanOldRequestsTests : RestApiIntegrationTestBase
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
    public async Task RestApiIntegration_CleanOldRequests_HappyFlow()
    {
        // Perform a lot of requests.
        for (var i = 0; i < 45; i++)
        {
            await Client.GetAsync($"{BaseAddress}some-request");
        }

        // Verify the number of performed requests.
        var requests = await GetRequestOverviewAsync();
        Assert.AreEqual(45, requests.Length);

        // Run the cleaning job.
        using var scheduledJobResponse = await Client.PostAsync($"{BaseAddress}ph-api/scheduledJob/CleanOldRequestsJob",
            new StringContent(string.Empty));
        scheduledJobResponse.EnsureSuccessStatusCode();

        // Verify the number of performed requests.
        requests = await GetRequestOverviewAsync();
        Assert.AreEqual(40, requests.Length);
    }

    private async Task<RequestOverviewDto[]> GetRequestOverviewAsync()
    {
        using var response = await Client.GetAsync($"{BaseAddress}ph-api/requests/overview");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<RequestOverviewDto[]>(content);
    }
}
