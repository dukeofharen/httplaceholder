using System.Text;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class DynamicResponseWriterFacts
{
    private readonly Mock<IResponseVariableParser> _variableParserMock = new();
    private DynamicResponseWriter _writer;

    [TestInitialize]
    public void Initialize() => _writer = new DynamicResponseWriter(_variableParserMock.Object);

    [TestCleanup]
    public void Cleanup() => _variableParserMock.VerifyAll();

    [TestMethod]
    public async Task DynamicResponseWriter_WriteToResponseAsync_EnableDynamicModeIsFalse_ShouldReturnFalse()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { EnableDynamicMode = false } };
        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsFalse(result.Executed);
    }

    [TestMethod]
    public async Task DynamicResponseWriter_WriteToResponseAsync_NoBodyAndHeaders_ShouldNotCallParse()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { EnableDynamicMode = true } };
        var response = new ResponseModel();

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        _variableParserMock.Verify(m => m.ParseAsync(It.IsAny<string>(), stub, It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DynamicResponseWriter_WriteToResponseAsync_OnlyBodySet_ShouldParseBody()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { EnableDynamicMode = true } };
        const string body = "this is the body";
        var response = new ResponseModel { Body = Encoding.UTF8.GetBytes(body) };

        _variableParserMock
            .Setup(m => m.ParseAsync(It.IsAny<string>(), stub, It.IsAny<CancellationToken>()))
            .Returns<string, StubModel, CancellationToken>((i, _, _) => Task.FromResult(i));

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        _variableParserMock.Verify(m => m.ParseAsync(body, stub, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task DynamicResponseWriter_WriteToResponseAsync_OnlyBodySet_BodyIsBinary_ShouldNotParseBody()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { EnableDynamicMode = true } };
        var response = new ResponseModel { Body = [1, 2, 3], BodyIsBinary = true };

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        _variableParserMock.Verify(m => m.ParseAsync(It.IsAny<string>(), stub, It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [TestMethod]
    public async Task DynamicResponseWriter_WriteToResponseAsync_BodyAndHeadersSet_ShouldParseBodyAndHeaders()
    {
        // arrange
        var stub = new StubModel { Response = new StubResponseModel { EnableDynamicMode = true } };
        const string body = "this is the body";
        var response = new ResponseModel
        {
            Body = Encoding.UTF8.GetBytes(body),
            Headers = { { "X-Header-1", "Header1" }, { "X-Header-2", "Header2" } }
        };

        _variableParserMock
            .Setup(m => m.ParseAsync(It.IsAny<string>(), stub, It.IsAny<CancellationToken>()))
            .Returns<string, StubModel, CancellationToken>((i, _, _) => Task.FromResult(i));

        // act
        var result = await _writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // assert
        Assert.IsTrue(result.Executed);
        _variableParserMock.Verify(m => m.ParseAsync(body, stub, It.IsAny<CancellationToken>()), Times.Once);
        _variableParserMock.Verify(m => m.ParseAsync("Header1", stub, It.IsAny<CancellationToken>()), Times.Once);
        _variableParserMock.Verify(m => m.ParseAsync("Header2", stub, It.IsAny<CancellationToken>()), Times.Once);
    }
}
