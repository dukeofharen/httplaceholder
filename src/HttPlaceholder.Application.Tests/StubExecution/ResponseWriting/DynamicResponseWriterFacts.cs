using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Application.StubExecution.VariableHandling;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class DynamicResponseWriterFacts
    {
        private readonly Mock<IVariableParser> _variableParserMock = new Mock<IVariableParser>();
        private DynamicResponseWriter _writer;

        [TestInitialize]
        public void Initialize() => _writer = new DynamicResponseWriter(_variableParserMock.Object);

        [TestCleanup]
        public void Cleanup() => _variableParserMock.VerifyAll();

        [TestMethod]
        public async Task DynamicResponseWriter_WriteToResponseAsync_EnableDynamicModeIsFalse_ShouldReturnFalse()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    EnableDynamicMode = false
                }
            };
            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DynamicResponseWriter_WriteToResponseAsync_NoBodyAndHeaders_ShouldNotCallParse()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    EnableDynamicMode = true
                }
            };
            var response = new ResponseModel();

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            _variableParserMock.Verify(m => m.Parse(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task DynamicResponseWriter_WriteToResponseAsync_OnlyBodySet_ShouldParseBody()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    EnableDynamicMode = true
                }
            };
            const string body = "this is the body";
            var response = new ResponseModel
            {
                Body = Encoding.UTF8.GetBytes(body)
            };

            _variableParserMock
                .Setup(m => m.Parse(It.IsAny<string>()))
                .Returns<string>(i => i);

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            _variableParserMock.Verify(m => m.Parse(body), Times.Once);
        }

        [TestMethod]
        public async Task DynamicResponseWriter_WriteToResponseAsync_OnlyBodySet_BodyIsBinary_ShouldNotParseBody()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    EnableDynamicMode = true
                }
            };
            var response = new ResponseModel
            {
                Body = new byte[] { 1, 2, 3 },
                BodyIsBinary = true
            };

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            _variableParserMock.Verify(m => m.Parse(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task DynamicResponseWriter_WriteToResponseAsync_BodyAndHeadersSet_ShouldParseBodyAndHeaders()
        {
            // arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    EnableDynamicMode = true
                }
            };
            const string body = "this is the body";
            var response = new ResponseModel
            {
                Body = Encoding.UTF8.GetBytes(body),
                Headers =
                {
                    { "X-Header-1", "Header1" },
                    { "X-Header-2", "Header2" }
                }
            };

            _variableParserMock
                .Setup(m => m.Parse(It.IsAny<string>()))
                .Returns<string>(i => i);

            // act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // assert
            Assert.IsTrue(result);
            _variableParserMock.Verify(m => m.Parse(body), Times.Once);
            _variableParserMock.Verify(m => m.Parse("Header1"), Times.Once);
            _variableParserMock.Verify(m => m.Parse("Header2"), Times.Once);
        }
    }
}
