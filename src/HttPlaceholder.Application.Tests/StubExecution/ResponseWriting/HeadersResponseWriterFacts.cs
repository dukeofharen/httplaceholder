using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class HeadersResponseWriterFacts
    {
        private readonly HeadersResponseWriter _writer = new HeadersResponseWriter();

        [TestMethod]
        public async Task HeadersResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Headers = null
                }
            };

            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, response.Headers.Count);
        }

        [TestMethod]
        public async Task HeadersResponseWriter_WriteToResponseAsync_HappyFlow()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    Headers = new Dictionary<string, string>
               {
                  { "X-Api-Key", "1223" },
                  { "X-User-Secret", "abc" }
               }
                }
            };

            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, response.Headers.Count);
            Assert.AreEqual("1223", response.Headers["X-Api-Key"]);
            Assert.AreEqual("abc", response.Headers["X-User-Secret"]);
        }
    }
}
