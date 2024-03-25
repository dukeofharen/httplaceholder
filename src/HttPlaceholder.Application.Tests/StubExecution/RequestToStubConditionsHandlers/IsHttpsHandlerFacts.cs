using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class IsHttpsHandlerFacts
{
    private readonly IsHttpsHandler _handler = new();

    [TestMethod]
    public async Task IsHttpsHandler_HandleStubGenerationAsync_NoHttps_ShouldNotSetIsHttps()
    {
        // Arrange
        var request = new HttpRequestModel { Url = "http://httplaceholder.com" };
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsFalse(conditions.Url.IsHttps.HasValue);
    }

    [TestMethod]
    public async Task IsHttpsHandler_HandleStubGenerationAsync_Https_ShouldSetToTrue()
    {
        // Arrange
        var request = new HttpRequestModel { Url = "https://httplaceholder.com" };
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(conditions.Url.IsHttps.HasValue && conditions.Url.IsHttps.Value);
    }
}
