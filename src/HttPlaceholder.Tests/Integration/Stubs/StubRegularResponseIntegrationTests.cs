﻿using System.Net;
using System.Net.Http;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubRegularResponseIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularGet_Host()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}client-ip-1";
        var request = new HttpRequestMessage { RequestUri = new Uri(url) };
        request.Headers.Add("X-Forwarded-Host", "httplaceholder.com");
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
    public async Task StubIntegration_RegularGet_Base64Content_HappyFlow()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}image.jpg";

        // act / assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsByteArrayAsync();
        Assert.AreEqual(632, content.Length);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_RegularGet_Html()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}index.html";

        // act / assert
        using var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(content.Contains("Test page in HttPlaceholder"));
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.HtmlMime, response.Content.Headers.ContentType.ToString());
    }
}
