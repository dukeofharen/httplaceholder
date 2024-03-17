using HttPlaceholder.Application.StubExecution.ResponseWriters;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseWriters;

[TestClass]
public class AbortConnectionResponseWriterFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task WriteToResponseAsync_AbortConnectionNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var writer = _mocker.CreateInstance<AbortConnectionResponseWriter>();

        var stub = new StubModel { Response = new StubResponseModel { AbortConnection = false } };
        var response = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsFalse(result.Executed);
        Assert.IsFalse(response.AbortConnection);
    }

    [TestMethod]
    public async Task WriteToResponseAsync_AbortConnectionSet_ShouldReturnExecuted()
    {
        // Arrange
        var writer = _mocker.CreateInstance<AbortConnectionResponseWriter>();

        var stub = new StubModel { Response = new StubResponseModel { AbortConnection = true } };
        var response = new ResponseModel();

        // Act
        var result = await writer.WriteToResponseAsync(stub, response, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Executed);
        Assert.IsTrue(response.AbortConnection);
    }
}
