using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.RestApi
{
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
            var content = await File.ReadAllTextAsync("chrome_on_ubuntu_multiple_curls.txt");

            // Post cURL commands to API.
            var url = $"{BaseAddress}ph-api/import/curl?doNotCreateStub=false";
            var apiRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = new StringContent(content, Encoding.UTF8, "text/plain")
            };
            var curlResponse = await Client.SendAsync(apiRequest);
            curlResponse.EnsureSuccessStatusCode();

            // Get and check the stubs.
            var stubs = StubSource.StubModels.ToArray();

            // Assert stubs.
            Assert.AreEqual(3, stubs.Length);

            var stub1 = stubs[0];
            Assert.AreEqual("generated-9244dcb5e6fb8259f92bf9d89eeb5d4f", stub1.Id);
            Assert.AreEqual("GET", stub1.Conditions.Method);
            Assert.AreEqual("/_nuxt/fonts/fa-solid-900.3eb06c7.woff2", stub1.Conditions.Url.Path);
            var headers1 = stub1.Conditions.Headers;
            Assert.AreEqual(2, headers1.Count);
            Assert.AreEqual("https://site\\.com", headers1["Origin"]);
            Assert.AreEqual("deflate,\\ gzip,\\ br", headers1["Accept-Encoding"]);

            var stub2 = stubs[1];
            Assert.AreEqual("generated-56f4d8c9af8353dec743938a9775bafd", stub2.Id);
            Assert.AreEqual("GET", stub2.Conditions.Method);
            Assert.AreEqual("/_nuxt/css/4cda201.css", stub2.Conditions.Url.Path);
            var headers2 = stub2.Conditions.Headers;
            Assert.AreEqual(4, headers2.Count);
            Assert.AreEqual("site\\.com", headers2["authority"]);
            Assert.AreEqual("en-US,en;q=0\\.9,nl;q=0\\.8", headers2["accept-language"]);
            Assert.AreEqual("Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=", headers2["cookie"]);
            Assert.AreEqual("deflate,\\ gzip,\\ br", headers2["Accept-Encoding"]);

            var stub3 = stubs[2];
            Assert.AreEqual("generated-87c52adc29dc4ea337978afb2b7649c7", stub3.Id);
            Assert.AreEqual("GET", stub3.Conditions.Method);
            Assert.AreEqual("/_nuxt/1d6c3a9.js", stub3.Conditions.Url.Path);
            var headers3 = stub3.Conditions.Headers;
            Assert.AreEqual(3, headers3.Count);
            Assert.AreEqual("site\\.com", headers3["authority"]);
            Assert.AreEqual("Consent=eyJhbmFseXRpY2FsIjpmYWxzZX0=", headers3["cookie"]);
            Assert.AreEqual("deflate,\\ gzip,\\ br", headers3["Accept-Encoding"]);
        }
    }
}
