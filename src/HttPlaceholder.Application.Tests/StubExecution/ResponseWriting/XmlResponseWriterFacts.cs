using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class XmlResponseWriterFacts
    {
        private XmlResponseWriter _writer = new XmlResponseWriter();

        [TestMethod]
        public async Task XmlResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Xml = null
                }
            };

            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public async Task XmlResponseWriter_WriteToResponseAsync_HappyFlow()
        {
            // arrange
            var responseText = "<xml>";
            var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Xml = responseText
                }
            };

            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
            Assert.AreEqual("text/xml", response.Headers["Content-Type"]);
        }

        [TestMethod]
        public async Task XmlResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
        {
            // arrange
            var responseText = "<xml>";
            var expectedResponseBytes = Encoding.UTF8.GetBytes(responseText);
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Xml = responseText
                }
            };

            var response = new ResponseModel();
            response.Headers.Add("Content-Type", "text/plain");

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(expectedResponseBytes.SequenceEqual(expectedResponseBytes));
            Assert.AreEqual("text/plain", response.Headers["Content-Type"]);
        }
    }
}
