﻿using System.Net;
using System.Net.Http;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubClientIpConditionsIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularGet_ClientIp_SingleAddress()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}client-ip-1";
        var request = new HttpRequestMessage { RequestUri = new Uri(url) };

        ClientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns("127.0.0.1");

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("client-ip-1 OK", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_ClientIp_IpRange()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}client-ip-2";
        var request = new HttpRequestMessage { RequestUri = new Uri(url) };

        ClientDataResolverMock
            .Setup(m => m.GetClientIp())
            .Returns("127.0.0.5");

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("client-ip-2 OK", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
