﻿using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class TextResponseWriterFacts
    {
        private readonly TextResponseWriter _writer = new TextResponseWriter();

        [TestMethod]
        public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Text = null
                }
            };

            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result.Executed);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow()
        {
            // arrange
            const string text = "bla123";
            var expectedBody = Encoding.UTF8.GetBytes(text);
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Text = text
                }
            };

            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result.Executed);
            Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
            Assert.AreEqual("text/plain", response.Headers["Content-Type"]);
        }

        [TestMethod]
        public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
        {
            // arrange
            const string text = "bla123";
            var expectedBody = Encoding.UTF8.GetBytes(text);
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Text = text
                }
            };

            var response = new ResponseModel();
            response.Headers.Add("Content-Type", "text/xml");

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result.Executed);
            Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
            Assert.AreEqual("text/xml", response.Headers["Content-Type"]);
        }
    }
}
