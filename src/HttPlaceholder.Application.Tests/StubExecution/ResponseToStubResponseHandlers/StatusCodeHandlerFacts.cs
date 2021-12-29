using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseToStubResponseHandlers;

[TestClass]
public class StatusCodeHandlerFacts
{
    private readonly StatusCodeHandler _handler = new();

    [TestMethod]
    public async Task HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        var response = new HttpResponseModel {StatusCode = 204};
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(response.StatusCode, stubResponse.StatusCode);
    }
}
