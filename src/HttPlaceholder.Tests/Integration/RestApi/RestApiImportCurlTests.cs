using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace HttPlaceholder.Tests.Integration.RestApi;

[TestClass]
public class RestApiImportCurlTests : RestApiIntegrationTestBase
{
    [TestInitialize]
    public void Initialize() => InitializeRestApiIntegrationTest();

    [TestCleanup]
    public void Cleanup() => CleanupRestApiIntegrationTest();

    [TestMethod]
    public async Task RestApiIntegration_Import_ImportCurl_HappyFlow()
    {
        // Arrange
        var content = await File.ReadAllTextAsync("Resources/chrome_on_ubuntu_multiple_curls.txt");

        // Post cURL commands to API.
        var url = $"{BaseAddress}ph-api/import/curl?doNotCreateStub=false&tenant=tenant1";
        var apiRequest = new HttpRequestMessage
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Post,
            Content = new StringContent(content, Encoding.UTF8, Constants.TextMime)
        };
        var curlResponse = await Client.SendAsync(apiRequest);
        curlResponse.EnsureSuccessStatusCode();

        // Get and check the stubs.
        var stubs = StubSource.StubModels.ToArray();

        // Assert stubs.
        Assert.AreEqual(3, stubs.Length);

        var stub1 = stubs[0];
        Assert.AreEqual("generated-9a9df959f3df979a2337b3d1e4829816", stub1.Id);
        Assert.AreEqual("GET", stub1.Conditions.Method);
        Assert.AreEqual("/_nuxt/fonts/fa-solid-900.3eb06c7.woff2",
            ((StubConditionStringCheckingModel)stub1.Conditions.Url.Path).StringEquals);
        var headers1 = stub1.Conditions.Headers;
        Assert.AreEqual(2, headers1.Count);
        Assert.AreEqual("https://site.com", ((StubConditionStringCheckingModel)headers1["Origin"]).StringEquals);
        Assert.AreEqual("deflate, gzip, br",
            ((StubConditionStringCheckingModel)headers1[Constants.AcceptEncoding]).StringEquals);

        var stub2 = stubs[1];
        Assert.AreEqual("generated-b19dfae4c2040bb2035c5b285a7f3232", stub2.Id);
        Assert.AreEqual("GET", stub2.Conditions.Method);
        Assert.AreEqual("/_nuxt/css/4cda201.css",
            ((StubConditionStringCheckingModel)stub2.Conditions.Url.Path).StringEquals);
        var headers2 = stub2.Conditions.Headers;
        Assert.AreEqual(4, headers2.Count);
        Assert.AreEqual("site.com", ((StubConditionStringCheckingModel)headers2["authority"]).StringEquals);
        Assert.AreEqual("en-US,en;q=0.9,nl;q=0.8",
            ((StubConditionStringCheckingModel)headers2["accept-language"]).StringEquals);
        Assert.AreEqual("Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=",
            ((StubConditionStringCheckingModel)headers2["cookie"]).StringEquals);
        Assert.AreEqual("deflate, gzip, br",
            ((StubConditionStringCheckingModel)headers2[Constants.AcceptEncoding]).StringEquals);

        var stub3 = stubs[2];
        Assert.AreEqual("generated-ded44f6ed44a83fc6086ee0ee0e7ce4b", stub3.Id);
        Assert.AreEqual("GET", stub3.Conditions.Method);
        Assert.AreEqual("/_nuxt/1d6c3a9.js",
            ((StubConditionStringCheckingModel)stub3.Conditions.Url.Path).StringEquals);
        var headers3 = stub3.Conditions.Headers;
        Assert.AreEqual(3, headers3.Count);
        Assert.AreEqual("site.com", ((StubConditionStringCheckingModel)headers3["authority"]).StringEquals);
        Assert.AreEqual("Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=",
            ((StubConditionStringCheckingModel)headers3["cookie"]).StringEquals);
        Assert.AreEqual("deflate, gzip, br",
            ((StubConditionStringCheckingModel)headers3[Constants.AcceptEncoding]).StringEquals);
    }
}
