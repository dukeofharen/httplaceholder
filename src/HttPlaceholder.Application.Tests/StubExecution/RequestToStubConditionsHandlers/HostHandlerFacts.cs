using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class HostHandlerFacts
{
    private readonly HostHandler _handler = new();

    [TestMethod]
    public async Task HostHandler_HandleStubGenerationAsync_Port80_NoPortInHost()
    {
        // Arrange
        var request = new HttpRequestModel {Url = "http://httplaceholder.com"};
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("httplaceholder.com", ((StubConditionStringCheckingModel)conditions.Host).StringEquals);
    }

    [TestMethod]
    public async Task HostHandler_HandleStubGenerationAsync_Port443_NoPortInHost()
    {
        // Arrange
        var request = new HttpRequestModel {Url = "https://httplaceholder.com"};
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("httplaceholder.com", ((StubConditionStringCheckingModel)conditions.Host).StringEquals);
    }

    [TestMethod]
    public async Task HostHandler_HandleStubGenerationAsync_Port5000_PortInHost()
    {
        // Arrange
        var request = new HttpRequestModel {Url = "https://httplaceholder.com:5000"};
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions, CancellationToken.None);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("httplaceholder.com:5000", ((StubConditionStringCheckingModel)conditions.Host).StringEquals);
    }
}
