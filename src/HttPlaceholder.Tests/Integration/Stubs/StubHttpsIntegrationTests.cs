using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubHttpsIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularGet_IsHttps_Ok()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ishttps-ok";
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url)
        };
        ClientIpResolverMock
            .Setup(m => m.GetClientIp())
            .Returns("127.0.0.1");
        ClientIpResolverMock
            .Setup(m => m.IsHttps())
            .Returns(true);

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("ishttps-ok OK", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(Constants.TextMime, response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_IsHttps_Nok()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}ishttps-ok";
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url)
        };
        request.Headers.Add("X-Forwarded-Proto", "http");

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
