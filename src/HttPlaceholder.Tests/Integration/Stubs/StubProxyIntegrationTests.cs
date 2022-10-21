using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichardSzalay.MockHttp;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubProxyIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_Proxy_Get_ProxiesCorrectly()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}todoitems/todos/1";
        var request = new HttpRequestMessage {RequestUri = new Uri(url)};

        MockHttp
            .When(HttpMethod.Get, "https://jsonplaceholder.typicode.com/todos/1")
            .Respond(Constants.TextMime, "OK from Proxy");

        // Act / Assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK from Proxy", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_Proxy_GetWithQueryString_ProxiesCorrectly()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}todoitems/todos/1?key=val&key2=val2";
        var request = new HttpRequestMessage {RequestUri = new Uri(url)};

        MockHttp
            .When(HttpMethod.Get, "https://jsonplaceholder.typicode.com/todos/1?key=val&key2=val2")
            .Respond(Constants.TextMime, "OK from Proxy");

        // Act / Assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK from Proxy", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_Proxy_Post_ProxiesCorrectly()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}todoitems/todos";
        const string postContent = "this is the content";
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(postContent, Encoding.UTF8, Constants.TextMime)
        };

        MockHttp
            .When(HttpMethod.Post, "https://jsonplaceholder.typicode.com/todos")
            .WithContent(postContent)
            .Respond(Constants.TextMime, "OK from Proxy");

        // Act / Assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK from Proxy", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
