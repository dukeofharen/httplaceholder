using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    [TestClass]
    public class StubHttpsIntegrationTests : StubIntegrationTestBase
    {
        [TestInitialize]
        public void Initialize()
        {
            InitializeStubIntegrationTest("integration.yml");
        }

        [TestCleanup]
        public void Cleanup()
        {
            CleanupIntegrationTest();
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_IsHttps_Ok()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ishttps-ok";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url)
            };
            request.Headers.Add("X-Forwarded-Proto", "https");
            ClientIpResolverMock
                .Setup(m => m.GetClientIp())
                .Returns("127.0.0.1");

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual("OK", content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_IsHttps_Nok()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}ishttps-ok";
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url)
            };
            request.Headers.Add("X-Forwarded-Proto", "http");

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);
            }
        }
    }
}
