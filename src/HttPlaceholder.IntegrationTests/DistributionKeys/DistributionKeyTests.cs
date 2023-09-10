using System.Net;
using HttPlaceholder.Client;
using HttPlaceholder.Client.Dto.Stubs;
using HttPlaceholder.Client.StubBuilders;
using HttPlaceholder.IntegrationTests.Clients;
using static HttPlaceholder.Client.Utilities.DtoExtensions;

namespace HttPlaceholder.IntegrationTests.DistributionKeys;

[TestClass]
public class DistributionKeyTests : IntegrationTestBase
{
    // TODO later:
    // - DevelopmentMiddleware moet distribution key header uitlezen.
    // - Deze test wat refactoren.

    [TestMethod]
    public async Task DistributionKey_ShouldSplitStubsAndRequests()
    {
        // Arrange
        var devClient = GetService<DevelopmentClient>();
        var httpClient = GetService<HttpClient>();
        await devClient.SetDistributionKeyAsync(string.Empty);

        // Act
        var plainClient = GetHttplClient();
        var stub1 = await CreateStub(1, plainClient);
        var response = await httpClient.GetAsync($"{RootUrl}/path1");
        Assert.AreEqual("OK1", await response.Content.ReadAsStringAsync());

        // Arrange: change distribution key
        var customKey = Guid.NewGuid().ToString();
        await devClient.SetDistributionKeyAsync(customKey); // TODO dit kan straks weg als er naar de header wordt geluisterd in de applicatie.
        response = await httpClient.GetAsync($"{RootUrl}/path1");
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);

        // Act: create stub
        var client = GetHttplClient(customKey);
        var stub2 = await CreateStub(2, client);
        response = await httpClient.GetAsync($"{RootUrl}/path2");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("OK2", await response.Content.ReadAsStringAsync());
        await client.DeleteStubAsync(stub2.Stub.Id);

        // Assert: check requests
        var requests = (await client.GetAllRequestsAsync()).ToArray();
        Assert.AreEqual(2, requests.Length);
        Assert.AreEqual(1, requests.Count(r => r.ExecutingStubId?.Equals(stub2.Stub.Id) == true));
    }

    private async Task<FullStubDto> CreateStub(int i, IHttPlaceholderClient client) =>
        await client.CreateStubAsync(StubBuilder.Begin()
            .WithId($"stub{i}")
            .WithConditions(StubConditionBuilder.Begin()
                .WithPath(StartsWith($"/path{i}")))
            .WithResponse(StubResponseBuilder.Begin()
                .WithTextResponseBody($"OK{i}")));
}
