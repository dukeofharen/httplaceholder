using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class PathHandlerFacts
{
    private readonly PathHandler _handler = new();

    [TestMethod]
    public async Task PathHandler_HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        const string url = "https://httplaceholder.com/A/Path?query1=val1&query2=val2";
        var request = new HttpRequestModel { Url = url };
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("/A/Path", ((StubConditionStringCheckingModel)conditions.Url.Path).StringEquals);
    }
}
