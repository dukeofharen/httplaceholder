using System.IO;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class CurlToHttpRequestMapperFacts
    {
        private readonly AutoMocker _mocker = new();

        [TestCleanup]
        public void Cleanup() => _mocker.VerifyAll();

        [TestMethod]
        public void MapCurlCommandsToHttpRequest_FirefoxOnUbuntu()
        {
            // Arrange
            var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
            var command = File.ReadAllText("Resources/cURL/firefox_on_ubuntu.txt");

            // Act
            var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);

            var request = result[0];
            Assert.AreEqual("POST", request.Method);
            Assert.AreEqual("https://api.site.com/api/v1/users/authenticate", request.Url);
            Assert.AreEqual("{}", request.Body);
            Assert.AreEqual(13, request.Headers.Count);

            var headers = request.Headers;
            Assert.AreEqual("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:94.0) Gecko/20100101 Firefox/94.0", headers["User-Agent"]);
            Assert.AreEqual("application/json, text/plain, */*", headers["Accept"]);
            Assert.AreEqual("en-US,en;q=0.5", headers["Accept-Language"]);
            Assert.AreEqual("application/json;charset=utf-8", headers["Content-Type"]);
            Assert.AreEqual("Basic ZHVjbzpibGFkaWJsYQ==", headers["Authorization"]);
            Assert.AreEqual("https://site.com", headers["Origin"]);
            Assert.AreEqual("keep-alive", headers["Connection"]);
            Assert.AreEqual("empty", headers["Sec-Fetch-Dest"]);
            Assert.AreEqual("cors", headers["Sec-Fetch-Mode"]);
            Assert.AreEqual("same-site", headers["Sec-Fetch-Site"]);
            Assert.AreEqual("1", headers["DNT"]);
            Assert.AreEqual("1", headers["Sec-GPC"]);
            Assert.AreEqual("trailers", headers["TE"]);
        }

        [TestMethod]
        public void mapCurlCommandsToHttpRequest_ChromeOnUbuntuSingle()
        {
            // Arrange
            var mapper = _mocker.CreateInstance<CurlToHttpRequestMapper>();
            var command = File.ReadAllText("Resources/cURL/chrome_on_ubuntu_single.txt");

            // Act
            var result = mapper.MapCurlCommandsToHttpRequest(command).ToArray();

            // Assert
            Assert.AreEqual(1, result.Length);

            var request = result[0];
            Assert.AreEqual("POST", request.Method);
            Assert.AreEqual("https://api.site.com/api/v1/users/authenticate", request.Url);
            Assert.AreEqual("{}", request.Body);
            Assert.AreEqual(13, request.Headers.Count);

            var headers = request.Headers;
            Assert.AreEqual("api.site.com", headers["authority"]);
            Assert.AreEqual(@""" Not A;Brand"";v=""99"", ""Chromium"";v=""96"", ""Google Chrome"";v=""96""", headers["sec-ch-ua"]);
            Assert.AreEqual("application/json, text/plain, */*", headers["accept"]);
            Assert.AreEqual("application/json;charset=UTF-8", headers["content-type"]);
            Assert.AreEqual("Basic dXNlcjpwYXNz", headers["authorization"]);
            Assert.AreEqual("?0", headers["sec-ch-ua-mobile"]);
            Assert.AreEqual("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.45 Safari/537.36", headers["user-agent"]);
            Assert.AreEqual(@"""Linux""", headers["sec-ch-ua-platform"]);
            Assert.AreEqual("https://site.com", headers["origin"]);
            Assert.AreEqual("same-site", headers["sec-fetch-site"]);
            Assert.AreEqual("cors", headers["sec-fetch-mode"]);
            Assert.AreEqual("empty", headers["sec-fetch-dest"]);
            Assert.AreEqual("en-US,en;q=0.9,nl;q=0.8", headers["accept-language"]);
        }
    }
}
