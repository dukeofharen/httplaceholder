using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseToStubResponseHandlers;

[TestClass]
public class HeaderHandlerFacts
{
    private readonly HeaderHandler _handler = new();

    [TestMethod]
    public async Task HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        var response = new HttpResponseModel {Headers = {{"Content-Type", "application/json"}}};
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(response.Headers, stubResponse.Headers);
    }
}
