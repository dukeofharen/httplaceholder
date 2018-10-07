using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.BusinessLogic.Implementations.ResponseWriters;
using HttPlaceholder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.BusinessLogic.Tests.Implementations.ResponseWriters
{
    [TestClass]
    public class HtmlResponseWriterFacts
    {
        private HtmlResponseWriter _writer;

        [TestInitialize]
        public void Initialize()
        {
            _writer = new HtmlResponseWriter();
        }

        [TestMethod]
        public async Task HtmlResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Html = null
                }
            };

            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public async Task HtmlResponseWriter_WriteToResponseAsync_HappyFlow()
        {
            // arrange
            string responseText = "<html>";
            var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Html = responseText
                }
            };

            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
            Assert.AreEqual("text/html", response.Headers["Content-Type"]);
        }

        [TestMethod]
        public async Task HtmlResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
        {
            // arrange
            string responseText = "<html>";
            var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Html = responseText
                }
            };

            var response = new ResponseModel();
            response.Headers.Add("Content-Type", "text/plain");

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
            Assert.AreEqual("text/plain", response.Headers["Content-Type"]);
        }
    }
}