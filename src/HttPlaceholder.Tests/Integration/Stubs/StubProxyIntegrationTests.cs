using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
        var request = new HttpRequestMessage { RequestUri = new Uri(url) };

        MockHttp
            .When(HttpMethod.Get, "https://example.com/todos/1")
            .Respond(HttpStatusCode.OK, new Dictionary<string, string> { { "X-Header", "value-from-proxy" } },
                MimeTypes.TextMime, "OK from Proxy");

        // Act / Assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK from Proxy", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("value-from-stub", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
    }

    [TestMethod]
    public async Task StubIntegration_Proxy_GetWithQueryString_ProxiesCorrectly()
    {
        // Arrange
        var url = $"{TestServer.BaseAddress}todoitems/todos/1?key=val&key2=val2";
        var request = new HttpRequestMessage { RequestUri = new Uri(url) };

        MockHttp
            .When(HttpMethod.Get, "https://example.com/todos/1?key=val&key2=val2")
            .Respond(MimeTypes.TextMime, "OK from Proxy");

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
            Content = new StringContent(postContent, Encoding.UTF8, MimeTypes.TextMime)
        };

        MockHttp
            .When(HttpMethod.Post, "https://example.com/todos")
            .WithContent(postContent)
            .Respond(MimeTypes.TextMime, "OK from Proxy");

        // Act / Assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("OK from Proxy", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }
}
