﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiImportHarTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Import_ImportHar_HappyFlow()
    {
        // Arrange
        var content = await File.ReadAllTextAsync("Resources/har.txt");

        // Post HAR to API.
        var url = $"{BaseAddress}ph-api/import/har?doNotCreateStub=false&tenant=tenant1";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, Constants.TextMime)
        };
        var harResponse = await Client.SendAsync(apiRequest);
        harResponse.EnsureSuccessStatusCode();

        // Get and check the stubs.
        var stubs = StubSource.StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(5, stubs.Length);

        var stub = stubs[0];
        Assert.AreEqual("generated-24f00eecf5439f6f305a168b184339d3", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/", stub.Conditions.Url.Path);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(14, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache", stub.Conditions.Headers["cache-control"]);
        Assert.AreEqual("ducode.org", stub.Conditions.Host);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual(Constants.HtmlMime, stub.Response.ContentType);
        Assert.AreEqual(4, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Html.Contains("<!DOCTYPE html>"));

        stub = stubs[1];
        Assert.AreEqual("generated-09b1935bc83ba1afcfdfe613dcdcd2e3", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/static/style/style.css", stub.Conditions.Url.Path);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(13, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache", stub.Conditions.Headers["cache-control"]);
        Assert.AreEqual("ducode.org", stub.Conditions.Host);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual("text/css", stub.Response.ContentType);
        Assert.AreEqual(5, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Text.Contains("@font-face"));

        stub = stubs[2];
        Assert.AreEqual("generated-76cff71ff1aaa9532192f64cca2958b7", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/static/fonts/roboto-mono.woff2", stub.Conditions.Url.Path);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(14, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache", stub.Conditions.Headers["cache-control"]);
        Assert.AreEqual("ducode.org", stub.Conditions.Host);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual("application/octet-stream", stub.Response.ContentType);
        Assert.AreEqual(5, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Base64.Contains("d09GMgABAAAAADAYAA4AAAAAV5wAAC"));

        stub = stubs[3];
        Assert.AreEqual("generated-4b3af4b16f93185600d5b8803e8fd7b0", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/static/favicon.png", stub.Conditions.Url.Path);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(13, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache", stub.Conditions.Headers["cache-control"]);
        Assert.AreEqual("ducode.org", stub.Conditions.Host);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual("image/png", stub.Response.ContentType);
        Assert.AreEqual(5, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Base64.Contains("iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAA"));

        stub = stubs[4];
        Assert.AreEqual("generated-a5c900b24f4dc4a28df93416597420e1", stub.Id);
        Assert.AreEqual("PUT", stub.Conditions.Method);
        Assert.AreEqual("/api/v1/admin/users/123", stub.Conditions.Url.Path);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(15, stub.Conditions.Headers.Count);
        Assert.AreEqual("keep-alive", stub.Conditions.Headers["Connection"]);
        Assert.AreEqual("api.site.com", stub.Conditions.Host);
        Assert.AreEqual("Dukeofharen", ((JObject)stub.Conditions.Json)["firstName"].ToString());
        Assert.AreEqual(204, stub.Response.StatusCode);
        Assert.AreEqual(11, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.AreEqual(string.Empty, stub.Response.Text);
    }
}
