using System;
using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriting
{
    [TestClass]
    public class LineEndingResponseWriterFacts
    {
        private readonly LineEndingResponseWriter _writer =
            new LineEndingResponseWriter(new Mock<ILogger<LineEndingResponseWriter>>().Object);

        [TestMethod]
        public async Task WriteToResponseAsync_LineEndingsNotSet_ShouldReturnNotExecuted()
        {
            // Arrange
            var stub = new StubModel {Response = new StubResponseModel {LineEndings = null}};
            var response = new ResponseModel();

            // Act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsFalse(result.Executed);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_Unix_ShouldReturnUnixLineEndings()
        {
            // Arrange
            var stub = new StubModel {Response = new StubResponseModel {LineEndings = "unix"}};
            var response = new ResponseModel {Body = Encoding.UTF8.GetBytes("the\r\ncontent\r\n")};

            // Act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsTrue(result.Executed);
            var content = Encoding.UTF8.GetString(response.Body);
            Assert.AreEqual("the\ncontent\n", content);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_Windows_ShouldReturnWindowsLineEndings()
        {
            // Arrange
            var stub = new StubModel {Response = new StubResponseModel {LineEndings = "windows"}};
            var response = new ResponseModel {Body = Encoding.UTF8.GetBytes("the\ncontent\n")};

            // Act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsTrue(result.Executed);
            var content = Encoding.UTF8.GetString(response.Body);
            Assert.AreEqual("the\r\ncontent\r\n", content);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_UnknownLineEndings_ShouldNotReplaceLineEndings()
        {
            // Arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel {LineEndings = "unknown", Text = "the\ncontent\n"}
            };
            var response = new ResponseModel();

            // Act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsTrue(result.Executed);
            Assert.IsNull(response.Body);
            Assert.AreEqual("Line ending type 'unknown' is not supported. Options are 'unix' and 'windows'.",
                result.Log);
        }

        [TestMethod]
        public async Task WriteToResponseAsync_ContentIsBinary_ShouldNotReplaceLineEndings()
        {
            // Arrange
            var stub = new StubModel
            {
                Response = new StubResponseModel
                {
                    LineEndings = "unknown", Base64 = Convert.ToBase64String(new byte[] {1, 2, 3})
                }
            };
            var response = new ResponseModel {BodyIsBinary = true};

            // Act
            var result = await _writer.WriteToResponseAsync(stub, response);

            // Assert
            Assert.IsTrue(result.Executed);
            Assert.AreEqual("The response body is binary; cannot replace line endings.", result.Log);
        }
    }
}
