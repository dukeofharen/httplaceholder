using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class RedirectResponseWriterFacts
    {
        private RedirectResponseWriter _writer;

        [TestInitialize]
        public void Initialize()
        {
            _writer = new RedirectResponseWriter();
        }

        [TestMethod]
        public async Task RedirectResponseWriter_WriteToResponseAsync_NoRedirectSet_ShouldContinue()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    PermanentRedirect = null,
                    TemporaryRedirect = null
                }
            };

            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
            Assert.AreEqual(0, response.StatusCode);
        }

        [TestMethod]
        public async Task RedirectResponseWriter_WriteToResponseAsync_TempRedirect()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    TemporaryRedirect = "https://google.com"
                }
            };

            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(307, response.StatusCode);
            Assert.AreEqual("https://google.com", response.Headers["Location"]);
        }

        [TestMethod]
        public async Task RedirectResponseWriter_WriteToResponseAsync_PermanentRedirect()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    PermanentRedirect = "https://google.com"
                }
            };

            var response = new ResponseModel();

            // act
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(301, response.StatusCode);
            Assert.AreEqual("https://google.com", response.Headers["Location"]);
        }
    }
}
