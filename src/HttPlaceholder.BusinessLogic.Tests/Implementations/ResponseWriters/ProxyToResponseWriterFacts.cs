using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HttPlaceholder.BusinessLogic.Implementations.ResponseWriters;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ResponseWriters
{
    [TestClass]
    public class ProxyToResponseWriterFacts
    {
        private Mock<IHttpContextService> _httpContextServiceMock;
        private Mock<IWebService> _webServiceMock;
        private ProxyToResponseWriter _writer;

        [TestInitialize]
        public void Initialize()
        {
            _httpContextServiceMock = new Mock<IHttpContextService>();
            _webServiceMock = new Mock<IWebService>();
            _writer = new ProxyToResponseWriter(
                _httpContextServiceMock.Object,
                _webServiceMock.Object);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpContextServiceMock.VerifyAll();
            _webServiceMock.VerifyAll();
        }

        [TestMethod]
        public async Task ProxyToResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    ProxyTo = null
                }
            };
            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task ProxyToResponseWriter_WriteToResponseAsync_HappyFlow_Post()
        {
            // arrange
            HttpRequestMessage actualRequest = null;
            string proxyUrl = "https://httplaceholder.com";
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    ProxyTo = proxyUrl
                }
            };
            var response = new ResponseModel();

            string method = "POST";
            var body = new byte[] { 1, 2, 3 };
            var headers = new Dictionary<string, string>
            {
                { "Content-Type", "text/plain" },
                { "Content-Length", "3" },
                { "Host", "localhost" },
                { "X-Api-Key", "1122" }
            };

            _httpContextServiceMock
                .Setup(m => m.Method)
                .Returns(method);
            _httpContextServiceMock
                .Setup(m => m.GetBodyAsBytes())
                .Returns(body);
            _httpContextServiceMock
                .Setup(m => m.GetHeaders())
                .Returns(headers);

            var responseContent = new byte[] { 3, 2, 1 };
            int responseStatusCode = 200;
            var webResponse = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)responseStatusCode,
                Content = new ByteArrayContent(responseContent)
            };
            webResponse.Content.Headers.Add("Content-Type", "text/xml");
            webResponse.Headers.Add("X-HttPlaceholder-Correlation", "3311");
            webResponse.Headers.Add("Author", "Duco");
            _webServiceMock
                .Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(m => actualRequest = m)
                .ReturnsAsync(webResponse);

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);

            // Assert request.
            Assert.AreEqual(stub.Response.ProxyTo, actualRequest.RequestUri.OriginalString);
            Assert.AreEqual(method, actualRequest.Method.Method);

            var actualRequestContent = await actualRequest.Content.ReadAsByteArrayAsync();
            Assert.IsTrue(actualRequestContent.SequenceEqual(body));

            Assert.AreEqual("text/plain", actualRequest.Content.Headers.Single(h => h.Key == "Content-Type").Value.Single());
            Assert.AreEqual("3", actualRequest.Content.Headers.Single(h => h.Key == "Content-Length").Value.Single());
            Assert.AreEqual(1, actualRequest.Headers.Count());
            Assert.AreEqual("1122", actualRequest.Headers.Single(h => h.Key == "X-Api-Key").Value.Single());

            // Assert response.
            Assert.IsTrue(responseContent.SequenceEqual(response.Body));
            Assert.AreEqual(1, response.Headers.Count);
            Assert.AreEqual("Duco", response.Headers["Author"]);
            Assert.AreEqual(responseStatusCode, response.StatusCode);
        }

        [TestMethod]
        public async Task ProxyToResponseWriter_WriteToResponseAsync_HappyFlow_Get()
        {
            // arrange
            HttpRequestMessage actualRequest = null;
            string proxyUrl = "https://httplaceholder.com";
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    ProxyTo = proxyUrl
                }
            };
            var response = new ResponseModel();

            string method = "GET";
            var headers = new Dictionary<string, string>
            {
                { "Host", "localhost" },
                { "X-Api-Key", "1122" }
            };

            _httpContextServiceMock
                .Setup(m => m.Method)
                .Returns(method);
            _httpContextServiceMock
                .Setup(m => m.GetHeaders())
                .Returns(headers);

            var responseContent = new byte[] { 3, 2, 1 };
            int responseStatusCode = 200;
            var webResponse = new HttpResponseMessage
            {
                StatusCode = (HttpStatusCode)responseStatusCode,
                Content = new ByteArrayContent(responseContent)
            };
            webResponse.Content.Headers.Add("Content-Type", "text/xml");
            webResponse.Headers.Add("X-HttPlaceholder-Correlation", "3311");
            webResponse.Headers.Add("Author", "Duco");
            _webServiceMock
                .Setup(m => m.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(m => actualRequest = m)
                .ReturnsAsync(webResponse);

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);

            // Assert request.
            Assert.AreEqual(stub.Response.ProxyTo, actualRequest.RequestUri.OriginalString);
            Assert.AreEqual(method, actualRequest.Method.Method);

            Assert.AreEqual(1, actualRequest.Headers.Count());
            Assert.AreEqual("1122", actualRequest.Headers.Single(h => h.Key == "X-Api-Key").Value.Single());

            // Assert response.
            Assert.IsTrue(responseContent.SequenceEqual(response.Body));
            Assert.AreEqual(1, response.Headers.Count);
            Assert.AreEqual("Duco", response.Headers["Author"]);
            Assert.AreEqual(responseStatusCode, response.StatusCode);
        }
    }
}
