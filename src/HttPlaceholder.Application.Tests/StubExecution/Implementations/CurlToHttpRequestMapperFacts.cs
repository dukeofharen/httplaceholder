using System.IO;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class CurlToHttpRequestMapperFacts
    {
        private readonly CurlToHttpRequestMapper _mapper = new();

        [TestMethod]
        public void MapCurlCommandsToHttpRequest_FirefoxOnUbuntu()
        {
            // Arrange
            var command = File.ReadAllText("Resources/cURL/firefox_on_ubuntu.txt");

            // Arrange
            var result = (_mapper.MapCurlCommandsToHttpRequest(command)).ToArray();

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
    }
}
