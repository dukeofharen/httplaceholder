using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubHeadersIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_Headers_PresenceCheckSucceeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}headers-presence-check";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Headers = { { "Header-1", "somevalue" } }
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("headers-presence-check-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_Headers_PresenceCheckFails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}headers-presence-check";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Headers = { { "Header-1", "somevalue" }, { "Header-2", "somevalue" } }
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
