using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    [TestClass]
    public class StubBasicAuthIntegrationTests : StubIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize() => InitializeStubIntegrationTest("integration.yml");

        [TestCleanup]
        public void Cleanup() => CleanupIntegrationTest();

        [TestMethod]
        public async Task StubIntegration_RegularGet_BasicAuthentication()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}User.svc";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers = { { "Authorization", "Basic ZHVjbzpnZWhlaW0=" } }
            };

            // act / assert
            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsFalse(string.IsNullOrEmpty(content));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.AreEqual("application/xml", response.Content.Headers.ContentType.ToString());
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_BasicAuthentication_StubNotFound()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}User.svc";
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers = { { "Authorization", "Basic ZHVjbzpnZWhlaW0x" } }
            };

            // act / assert
            using var response = await Client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(string.IsNullOrEmpty(content));
            Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode);
        }
    }
}
