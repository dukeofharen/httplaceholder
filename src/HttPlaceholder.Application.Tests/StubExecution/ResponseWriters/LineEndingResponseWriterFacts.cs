using System.Text;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class LineEndingResponseWriterFacts
{
    private readonly LineEndingResponseWriter _writer = new();

    [TestMethod]
    public async Task WriteToResponseAsync_LineEndingsNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = new StubModel { Response = new StubResponseModel { LineEndings = null } };
        var response = new ResponseModel();

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_Unix_ShouldReturnUnixLineEndings()
    {
        // Arrange
        var stub = new StubModel { Response = new StubResponseModel { LineEndings = LineEndingType.Unix } };
        var response = new ResponseModel { Body = "the\r\ncontent\r\n"u8.ToArray() };

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Executed);
        var content = Encoding.UTF8.GetString(response.Body);
        Assert.AreEqual("the\ncontent\n", content);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_Windows_ShouldReturnWindowsLineEndings()
    {
        // Arrange
        var stub = new StubModel { Response = new StubResponseModel { LineEndings = LineEndingType.Windows } };
        var response = new ResponseModel { Body = "the\ncontent\n"u8.ToArray() };

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

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
            Response = new StubResponseModel { LineEndings = (LineEndingType)5, Text = "the\ncontent\n" }
        };
        var response = new ResponseModel();

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.IsNull(response.Body);
        Assert.AreEqual("Line ending type '5' is not supported. Options are: Unix,Windows",
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
                LineEndings = LineEndingType.Windows,
                Base64 = Convert.ToBase64String(new byte[] { 1, 2, 3 })
            }
        };
        var response = new ResponseModel { BodyIsBinary = true };

        // Act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.AreEqual("The response body is binary; cannot replace line endings.", result.Log);
    }
}
