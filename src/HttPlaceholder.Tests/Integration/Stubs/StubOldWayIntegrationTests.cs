using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubOldWayIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/oldway.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_OldWay_Path_Succeeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}path-old-way";
        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("path-old-way-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Path_Fails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}path-old-wa";
        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_FullPath_Succeeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}full-path-old-way?q1=val1";
        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("full-path-old-way-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_FullPath_Fails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}full-path-old-wa?q1=val1";
        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Query_Succeeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}query-old-way?q1=val1&q2=val2";
        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("query-old-way-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Query_Fails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}query-old-way?q1=val1&q2=val3";
        var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(url)};

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Body_Succeeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}body-old-way";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post, RequestUri = new Uri(url), Content = new StringContent("val1val2")
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("body-old-way-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Body_Fails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}body-old-way";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post, RequestUri = new Uri(url), Content = new StringContent("val1val3")
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Headers_Succeeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}headers-old-way";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Headers = {{"Header-1", "val1"}, {"Header-2", "val2"}}
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("headers-old-way-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Headers_Fails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}headers-old-way";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(url),
            Headers = {{"Header-1", "val1"}, {"Header-2", "val3"}}
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Form_Succeeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}form-old-way";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"key1", "val1"}, {"key2", "val2"}, {"key3", "val3"}
            })
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("form-old-way-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Form_Fails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}form-old-way";
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(url),
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"key1", "val1"}, {"key2", "val3"}, {"key3", "val3"}
            })
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Host_Succeeds()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}host-old-way";
        ClientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns("httplaceholder.com");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get, RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("host-old-way-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_OldWay_Host_Fails()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}host-old-way";
        ClientDataResolverMock
            .Setup(m => m.GetHost())
            .Returns("httplaceholder.net");
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get, RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
