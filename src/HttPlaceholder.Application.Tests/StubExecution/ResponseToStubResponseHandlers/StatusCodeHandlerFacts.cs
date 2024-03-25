using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseToStubResponseHandlers;

[TestClass]
public class StatusCodeHandlerFacts
{
    private readonly StatusCodeHandler _handler = new();

    [TestMethod]
    public async Task HandleStubGenerationAsync_StatusCodeSet_ShouldSetStatusCode()
    {
        // Arrange
        var response = new HttpResponseModel { StatusCode = 204 };
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(response.StatusCode, stubResponse.StatusCode);
    }

    [TestMethod]
    public async Task HandleStubGenerationAsync_StatusCodeNotSet_ShouldNotSetStatusCode()
    {
        // Arrange
        var response = new HttpResponseModel { StatusCode = 0 };
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(stubResponse.StatusCode);
    }
}
