using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
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
        var url = $"{BaseAddress}ph-api/import/har?doNotCreateStub=false&tenant=tenant1&stubIdPrefix=prefix-";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, MimeTypes.TextMime)
        };
        var harResponse = await Client.SendAsync(apiRequest);
        harResponse.EnsureSuccessStatusCode();

        // Get and check the stubs.
        var stubs = StubSource.StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(5, stubs.Length);

        var stub = stubs[0];
        Assert.AreEqual("prefix-generated-d6a8e9c4b6ed8afda1126a9e14ff71e1", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/", ((StubConditionStringCheckingModel)stub.Conditions.Url.Path).StringEquals);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(14, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache",
            ((StubConditionStringCheckingModel)stub.Conditions.Headers["cache-control"]).StringEquals);
        Assert.AreEqual("ducode.org", ((StubConditionStringCheckingModel)stub.Conditions.Host).StringEquals);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual(MimeTypes.HtmlMime, stub.Response.ContentType);
        Assert.AreEqual(4, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Html.Contains("<!DOCTYPE html>"));

        stub = stubs[1];
        Assert.AreEqual("prefix-generated-4ff30d3168e5d79036a66a05991b3f13", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/static/style/style.css",
            ((StubConditionStringCheckingModel)stub.Conditions.Url.Path).StringEquals);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(13, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache",
            ((StubConditionStringCheckingModel)stub.Conditions.Headers["cache-control"]).StringEquals);
        Assert.AreEqual("ducode.org", ((StubConditionStringCheckingModel)stub.Conditions.Host).StringEquals);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual("text/css", stub.Response.ContentType);
        Assert.AreEqual(5, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Text.Contains("@font-face"));

        stub = stubs[2];
        Assert.AreEqual("prefix-generated-cae4990d300653e0bacc8671356288d1", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/static/fonts/roboto-mono.woff2",
            ((StubConditionStringCheckingModel)stub.Conditions.Url.Path).StringEquals);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(14, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache",
            ((StubConditionStringCheckingModel)stub.Conditions.Headers["cache-control"]).StringEquals);
        Assert.AreEqual("ducode.org", ((StubConditionStringCheckingModel)stub.Conditions.Host).StringEquals);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual("application/octet-stream", stub.Response.ContentType);
        Assert.AreEqual(5, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Base64.Contains("d09GMgABAAAAADAYAA4AAAAAV5wAAC"));

        stub = stubs[3];
        Assert.AreEqual("prefix-generated-78d8806ad13ea193a0006c105a940884", stub.Id);
        Assert.AreEqual("GET", stub.Conditions.Method);
        Assert.AreEqual("/static/favicon.png",
            ((StubConditionStringCheckingModel)stub.Conditions.Url.Path).StringEquals);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(13, stub.Conditions.Headers.Count);
        Assert.AreEqual("no-cache",
            ((StubConditionStringCheckingModel)stub.Conditions.Headers["cache-control"]).StringEquals);
        Assert.AreEqual("ducode.org", ((StubConditionStringCheckingModel)stub.Conditions.Host).StringEquals);
        Assert.AreEqual(200, stub.Response.StatusCode);
        Assert.AreEqual("image/png", stub.Response.ContentType);
        Assert.AreEqual(5, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.IsTrue(stub.Response.Base64.Contains("iVBORw0KGgoAAAANSUhEUgAAAQAAAAEACAIAA"));

        stub = stubs[4];
        Assert.AreEqual("prefix-generated-f196612336094255a14208fd9ac8e839", stub.Id);
        Assert.AreEqual("PUT", stub.Conditions.Method);
        Assert.AreEqual("/api/v1/admin/users/123",
            ((StubConditionStringCheckingModel)stub.Conditions.Url.Path).StringEquals);
        Assert.IsTrue(stub.Conditions.Url.IsHttps);
        Assert.AreEqual(15, stub.Conditions.Headers.Count);
        Assert.AreEqual("keep-alive",
            ((StubConditionStringCheckingModel)stub.Conditions.Headers[HeaderKeys.Connection]).StringEquals);
        Assert.AreEqual("api.site.com", ((StubConditionStringCheckingModel)stub.Conditions.Host).StringEquals);
        Assert.AreEqual("Dukeofharen", ((JObject)stub.Conditions.Json)["firstName"].ToString());
        Assert.AreEqual(204, stub.Response.StatusCode);
        Assert.AreEqual(11, stub.Response.Headers.Count);
        Assert.AreEqual("nginx", stub.Response.Headers["server"]);
        Assert.AreEqual(string.Empty, stub.Response.Text);
    }

    [TestMethod]
    public async Task RestApiIntegration_Import_ImportHar_RequestHeaderSizeIsNull_ShouldWork()
    {
        // Arrange
        var content = await File.ReadAllTextAsync("Resources/har_headerssize_null.txt");

        // Post HAR to API.
        var url = $"{BaseAddress}ph-api/import/har?doNotCreateStub=false&tenant=tenant1";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, MimeTypes.TextMime)
        };
        var harResponse = await Client.SendAsync(apiRequest);
        harResponse.EnsureSuccessStatusCode();

        // Get and check the stubs.
        var stubs = StubSource.StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(1, stubs.Length);
    }
}
