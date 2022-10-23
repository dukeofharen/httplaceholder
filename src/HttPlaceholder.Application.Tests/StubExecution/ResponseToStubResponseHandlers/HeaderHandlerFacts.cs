using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseToStubResponseHandlers;

[TestClass]
public class HeaderHandlerFacts
{
    private readonly HeaderHandler _handler = new();

    [TestMethod]
    public async Task HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        var response = new HttpResponseModel {Headers = {{HeaderKeys.ContentType, Constants.JsonMime}}};
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(response.Headers, stubResponse.Headers);
    }
}
