using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Tests.Integration.Stubs
{
    [TestClass]
    public class StubDynamicModeIntegrationTests : StubIntegrationTestBase
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
        public async Task StubIntegration_RegularGet_Dynamic_Query()
        {
            // arrange
            string query1Val = "John";
            string query2Val = "=";
            string expectedResult = $"The value is {query1Val} {WebUtility.UrlEncode(query2Val)}";
            string url = $"{TestServer.BaseAddress}dynamic-query.txt?queryString1={query1Val}&queryString2={query2Val}";

            // act / assert
            using (var response = await Client.GetAsync(url))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_Uuid()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}dynamic-uuid.txt";

            // act / assert
            using (var response = await Client.GetAsync(url))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.IsTrue(Guid.TryParse(content.Replace("The value is ", string.Empty), out _));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_RequestHeaders()
        {
            // arrange
            string apiKey = Guid.NewGuid().ToString();
            string expectedResult = $"API key: {apiKey}";
            string url = $"{TestServer.BaseAddress}dynamic-request-header.txt";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-Api-Key", apiKey);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual("localhost", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularPost_Dynamic_FormPost()
        {
            // arrange
            string expectedResult = "Posted: Value 1!";
            string url = $"{TestServer.BaseAddress}dynamic-form-post.txt";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "formval1", "Value 1!" },
                    { "formval2", "Value 2!" }
                })
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual("Value 2!", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularPost_Dynamic_RequestBody()
        {
            // arrange
            string expectedResult = "Posted: Test123";
            string url = $"{TestServer.BaseAddress}dynamic-request-body.txt";
            string body = "Test123";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(body)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual("Test123", response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_DisplayUrl()
        {
            // arrange
            string query = "?var1=value1&var2=value2";
            string url = $"{TestServer.BaseAddress}dynamic-display-url.txt{query}";
            string expectedResult = $"URL: {url}";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual(url, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_ClientIp()
        {
            // arrange
            string ip = "11.22.33.44";
            string url = $"{TestServer.BaseAddress}dynamic-client-ip.txt";
            string expectedResult = $"IP: {ip}";

            ClientIpResolverMock
                .Setup(m => m.GetClientIp())
                .Returns(ip);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual(ip, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_LocalNow()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}dynamic-local-now.txt";
            string expectedDateTime = "2019-08-21 20:41:51";
            string expectedResult = $"Local now: {expectedDateTime}";

            var now = new DateTime(2019, 8, 21, 20, 41, 51, DateTimeKind.Local);
            DateTimeMock
                .Setup(m => m.Now)
                .Returns(now);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual(expectedDateTime, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_UtcNow()
        {
            // arrange
            string url = $"{TestServer.BaseAddress}dynamic-utc-now.txt";
            string expectedDateTime = "2019-08-21 20:41:51";
            string expectedResult = $"UTC now: {expectedDateTime}";

            var now = new DateTime(2019, 8, 21, 20, 41, 51, DateTimeKind.Utc);
            DateTimeMock
                .Setup(m => m.UtcNow)
                .Returns(now);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                string content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual(expectedDateTime, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }
    }
}
