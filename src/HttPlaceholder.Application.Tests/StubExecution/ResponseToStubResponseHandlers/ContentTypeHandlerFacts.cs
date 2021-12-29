using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.ResponseToStubResponseHandlers;

[TestClass]
public class ContentTypeHandlerFacts
{
    private readonly ContentTypeHandler _handler = new();

    [TestMethod]
    public async Task HandleStubGenerationAsync_NoContentTypeHeaderSet_ShouldReturnFalse()
    {
        // Arrange
        var response = new HttpResponseModel {Headers = {{"x-api-key", "1223"}}};
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(stubResponse.ContentType);
    }

    [DataTestMethod]
    [DataRow("content-type")]
    [DataRow("CONTENT-TYPE")]
    [DataRow("Content-Type")]
    public async Task HandleStubGenerationAsync_ContentTypeHeaderSet_ShouldReturnTrue(string headerKey)
    {
        // Arrange
        var response = new HttpResponseModel
        {
            Headers = {{"x-api-key", "1223"}, {headerKey, "application/json; charset=UTF-8"}}
        };
        var stubResponse = new StubResponseModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(response, stubResponse);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("application/json; charset=UTF-8", stubResponse.ContentType);
    }
}
