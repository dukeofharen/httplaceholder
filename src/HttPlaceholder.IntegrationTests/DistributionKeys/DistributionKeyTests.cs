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
    private IHttPlaceholderClient _httplClient;

    [TestMethod]
    public async Task DistributionKey_ShouldSplitStubsAndRequests()
    {
        // Arrange
        var devClient = GetService<DevelopmentClient>();
        var httpClient = GetService<HttpClient>();
        await devClient.SetDistributionKeyAsync(string.Empty);

        // Act
        var stub1 = await CreateStub(1);
        var response = await httpClient.GetAsync($"{RootUrl}/path1");
        Assert.AreEqual("OK1", await response.Content.ReadAsStringAsync());

        // Arrange: change distribution key
        var customKey = Guid.NewGuid().ToString();
        await devClient.SetDistributionKeyAsync(customKey);
        response = await httpClient.GetAsync($"{RootUrl}/path1");
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);

        // Act: create stub
        var stub2 = await CreateStub(2);
        response = await httpClient.GetAsync($"{RootUrl}/path2");
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("OK2", await response.Content.ReadAsStringAsync());
        await _httplClient.DeleteStubAsync(stub2.Stub.Id);

        // Assert: check requests
        var requests = (await _httplClient.GetAllRequestsAsync()).ToArray();
        Assert.AreEqual(2, requests.Length);
        Assert.AreEqual(1, requests.Count(r => r.ExecutingStubId?.Equals(stub2.Stub.Id) == true));
    }

    private async Task<FullStubDto> CreateStub(int i) =>
        await _httplClient.CreateStubAsync(StubBuilder.Begin()
            .WithId($"stub{i}")
            .WithConditions(StubConditionBuilder.Begin()
                .WithPath(StartsWith($"/path{i}")))
            .WithResponse(StubResponseBuilder.Begin()
                .WithTextResponseBody($"OK{i}")));

    public override void AfterInitialize() => _httplClient = GetService<IHttPlaceholderClient>();
}
