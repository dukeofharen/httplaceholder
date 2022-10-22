using System.Linq;
using System.Net;
using HttPlaceholder.Dto.v1.Requests;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiGeneralIntegrationTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_GetAllRequests_ShouldReturnResponseAsJsonByDefault()
    {
        // First, do a request to the stub for adding at least 1 request.
        var stubUrl = $"{TestServer.BaseAddress}non-existing-stub";
        using var stubResponse = await Client.GetAsync(stubUrl);
        Assert.AreEqual(HttpStatusCode.NotImplemented, stubResponse.StatusCode);

        // Then, fetch all requests from the API.
        var apiUrl = $"{TestServer.BaseAddress}ph-api/requests";
        using var apiResponse = await Client.GetAsync(apiUrl);
        Assert.AreEqual(HttpStatusCode.OK, apiResponse.StatusCode);
        var content = await apiResponse.Content.ReadAsStringAsync();

        // Verify that the response is JSON instead of YAML.
        Assert.IsFalse(content.StartsWith("- "));
        try
        {
            var parsed = JsonConvert.DeserializeObject<RequestResultDto[]>(content);
            var request = parsed.Single();
            Assert.IsFalse(string.IsNullOrWhiteSpace(request.CorrelationId));
        }
        catch
        {
            Assert.Fail("Request JSON could not be parsed correctly.");
        }
    }
}
