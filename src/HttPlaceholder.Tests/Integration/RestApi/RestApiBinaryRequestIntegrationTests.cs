using System.Linq;
using System.Net.Http;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;
using Newtonsoft.Json;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiBinaryRequestIntegrationTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Request_IsBinary()
    {
        // Act: perform binary request.
        using var requestResponse = await Client.PostAsync($"{TestServer.BaseAddress}binary",
            new ByteArrayContent(new byte[] {254, 255}));
        var correlation = requestResponse.Headers
            .Single(h => h.Key.Equals("X-HttPlaceholder-Correlation", StringComparison.OrdinalIgnoreCase)).Value
            .Single();

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RequestResultDto>(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("/v8=", result.RequestParameters.Body);
        Assert.IsTrue(result.RequestParameters.BodyIsBinary);
    }

    [TestMethod]
    public async Task RestApiIntegration_Request_IsNotBinary()
    {
        // Act: perform binary request.
        using var requestResponse =
            await Client.PostAsync($"{TestServer.BaseAddress}text", new StringContent("Some content"));
        var correlation = requestResponse.Headers
            .Single(h => h.Key.Equals("X-HttPlaceholder-Correlation", StringComparison.OrdinalIgnoreCase)).Value
            .Single();

        // Act
        using var response = await Client.GetAsync($"{TestServer.BaseAddress}ph-api/requests/{correlation}");
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RequestResultDto>(content);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Some content", result.RequestParameters.Body);
        Assert.IsFalse(result.RequestParameters.BodyIsBinary);
    }
}
