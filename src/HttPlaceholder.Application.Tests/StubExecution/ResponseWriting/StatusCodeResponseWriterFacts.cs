using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class StatusCodeResponseWriterFacts
    {
        private StatusCodeResponseWriter _writer = new StatusCodeResponseWriter();

        [TestMethod]
        public async Task StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow_NoValueSetInStub()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    StatusCode = null
                }
            };

            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(200, response.StatusCode);
        }

        [TestMethod]
        public async Task StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    StatusCode = 409
                }
            };

            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(409, response.StatusCode);
        }

        [TestMethod]
        public async Task StatusCodeResponseWriter_WriteToResponseAsync_HappyFlow_NoStatusCodeSetInStub_StatusCodeAlreadySetOnResponse_ShouldNotBeOverwritten()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    StatusCode = null
                }
            };

            var response = new ResponseModel
            {
                StatusCode = 409
            };

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(409, response.StatusCode);
        }
    }
}
