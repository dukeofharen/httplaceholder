using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class HeaderHandlerFacts
{
    private readonly HeaderHandler _handler = new();

    [TestMethod]
    public async Task HeaderHandler_HandleStubGenerationAsync_NoHeadersSet_ShouldReturnFalse()
    {
        // Arrange
        var request = new HttpRequestModel { Headers = new Dictionary<string, string>() };
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsFalse(result);
        Assert.IsFalse(conditions.Headers.Any());
    }

    [TestMethod]
    public async Task HeaderHandler_HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        var request = new HttpRequestModel
        {
            Headers = new Dictionary<string, string>
            {
                { HeaderKeys.PostmanToken, Guid.NewGuid().ToString() },
                { HeaderKeys.Host, "httplaceholder.com" },
                { "X-Api-Key", "123" },
                { "X-Bla", "bla" }
            }
        };
        var stub = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, stub, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(2, stub.Headers.Count);
        Assert.AreEqual("123", ((StubConditionStringCheckingModel)stub.Headers["X-Api-Key"]).StringEquals);
        Assert.AreEqual("bla", ((StubConditionStringCheckingModel)stub.Headers["X-Bla"]).StringEquals);
    }
}
