﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Tests.Integration.Stubs;

[TestClass]
public class StubPostBodyConditionsIntegrationTests : StubIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeStubIntegrationTest("Resources/integration.yml");

    [TestCleanup]
    public void Cleanup() => CleanupIntegrationTest();

    [TestMethod]
    public async Task StubIntegration_RegularPost_ValidatePostBody_HappyFlow()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}api/users";
        const string body = """{"username": "john"}""";
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body),
            Headers =
            {
                { "X-Api-Key", "123abc" },
                { "X-Another-Secret", "sjaaaaaak 123" },
                { "X-Another-Code", "Two Memories" }
            },
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.IsFalse(string.IsNullOrEmpty(content));
        JObject.Parse(content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("application/json", response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_ValidatePostBody_StubNotFound()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}api/users";
        const string body = """{"username": "jack"}""";
        var request = new HttpRequestMessage
        {
            Content = new StringContent(body),
            Headers = { { "X-Api-Key", "123abc" }, { "X-Another-Secret", "sjaaaaaak 123" } },
            Method = HttpMethod.Post,
            RequestUri = new Uri(url)
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_Form_Ok()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}form";
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key1", "sjaak"),
                new KeyValuePair<string, string>("key2", "bob"),
                new KeyValuePair<string, string>("key2", "ducoo")
            })
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("form-ok OK", content);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual(MimeTypes.TextMime, response.Content.Headers.ContentType.ToString());
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_Form_Nok()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}form";
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key1", "sjaak"),
                new KeyValuePair<string, string>("key2", "bob")
            })
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_PresentCheck_Ok()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}form";
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key1", "someval"),
                new KeyValuePair<string, string>("key3", "bob")
            })
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        Assert.AreEqual("form-present-check-ok", content);
    }

    [TestMethod]
    public async Task StubIntegration_RegularPost_PresentCheck_Nok()
    {
        // arrange
        var url = $"{TestServer.BaseAddress}form";
        var request = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key1", "someval"),
                new KeyValuePair<string, string>("key2", "should not be here"),
                new KeyValuePair<string, string>("key3", "bob")
            })
        };

        // act / assert
        using var response = await Client.SendAsync(request);
        Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
    }
}
