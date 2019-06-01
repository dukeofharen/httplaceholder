using System.Linq;
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
        private TextResponseWriter _writer;

        [TestInitialize]
        public void Initialize()
        {
            _writer = new TextResponseWriter();
        }

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
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
            Assert.IsNull(response.Body);
        }

        [TestMethod]
        public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow()
        {
            // arrange
            string text = "bla123";
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
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
            Assert.AreEqual("text/plain", response.Headers["Content-Type"]);
        }

        [TestMethod]
        public async Task TextResponseWriter_WriteToResponseAsync_HappyFlow_ContentTypeHeaderAlreadySet_HeaderShouldBeRespected()
        {
            // arrange
            string text = "bla123";
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
            bool result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            Assert.IsTrue(expectedBody.SequenceEqual(response.Body));
            Assert.AreEqual("text/xml", response.Headers["Content-Type"]);
        }
    }
}
