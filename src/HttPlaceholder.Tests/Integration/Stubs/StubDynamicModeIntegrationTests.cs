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
            var query1Val = "John";
            var query2Val = "=";
            var expectedResult = $"The value is {query1Val} {WebUtility.UrlEncode(query2Val)}";
            var url = $"{TestServer.BaseAddress}dynamic-query.txt?queryString1={query1Val}&queryString2={query2Val}";

            // act / assert
            using (var response = await Client.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_Uuid()
        {
            // arrange
            var url = $"{TestServer.BaseAddress}dynamic-uuid.txt";

            // act / assert
            using (var response = await Client.GetAsync(url))
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.IsTrue(Guid.TryParse(content.Replace("The value is ", string.Empty), out _));
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());
            }
        }

        [TestMethod]
        public async Task StubIntegration_RegularGet_Dynamic_RequestHeaders()
        {
            // arrange
            var apiKey = Guid.NewGuid().ToString();
            var expectedResult = $"API key: {apiKey}";
            var url = $"{TestServer.BaseAddress}dynamic-request-header.txt";

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("X-Api-Key", apiKey);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                var content = await response.Content.ReadAsStringAsync();
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
            var expectedResult = "Posted: Value 1!";
            var url = $"{TestServer.BaseAddress}dynamic-form-post.txt";

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
                var content = await response.Content.ReadAsStringAsync();
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
            var expectedResult = "Posted: Test123";
            var url = $"{TestServer.BaseAddress}dynamic-request-body.txt";
            var body = "Test123";

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(body)
            };

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                var content = await response.Content.ReadAsStringAsync();
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
            var query = "?var1=value1&var2=value2";
            var url = $"{TestServer.BaseAddress}dynamic-display-url.txt{query}";
            var expectedResult = $"URL: {url}";

            ClientIpResolverMock
                .Setup(m => m.GetHost())
                .Returns("localhost");

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                var content = await response.Content.ReadAsStringAsync();
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
            var ip = "11.22.33.44";
            var url = $"{TestServer.BaseAddress}dynamic-client-ip.txt";
            var expectedResult = $"IP: {ip}";

            ClientIpResolverMock
                .Setup(m => m.GetClientIp())
                .Returns(ip);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                var content = await response.Content.ReadAsStringAsync();
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
            var url = $"{TestServer.BaseAddress}dynamic-local-now.txt";
            var expectedDateTime = "2019-08-21 20:41:51";
            var expectedResult = $"Local now: {expectedDateTime}";

            var now = new DateTime(2019, 8, 21, 20, 41, 51, DateTimeKind.Local);
            DateTimeMock
                .Setup(m => m.Now)
                .Returns(now);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                var content = await response.Content.ReadAsStringAsync();
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
            var url = $"{TestServer.BaseAddress}dynamic-utc-now.txt";
            var expectedDateTime = "2019-08-21 20:41:51";
            var expectedResult = $"UTC now: {expectedDateTime}";

            var now = new DateTime(2019, 8, 21, 20, 41, 51, DateTimeKind.Utc);
            DateTimeMock
                .Setup(m => m.UtcNow)
                .Returns(now);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // act / assert
            using (var response = await Client.SendAsync(request))
            {
                var content = await response.Content.ReadAsStringAsync();
                Assert.AreEqual(expectedResult, content);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
                Assert.AreEqual("text/plain", response.Content.Headers.ContentType.ToString());

                Assert.AreEqual(expectedDateTime, response.Headers.Single(h => h.Key == "X-Header").Value.Single());
            }
        }
    }
}
