using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Application.Tests.StubExecution.RequestToStubConditionsHandlers;

[TestClass]
public class BodyHandlerFacts
{
    private readonly BodyHandler _handler = new();

    [TestMethod]
    public async Task BodyHandler_HandleStubGenerationAsync_JsonAlreadySetOnStub_ShouldReturnFalse()
    {
        // Arrange
        const string body = "POSTED!";
        var request = new HttpRequestModel {Body = body};
        var conditions = new StubConditionsModel {Json = new object()};

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task BodyHandler_HandleStubGenerationAsync_BodyNotSetOnRequest_ShouldReturnFalse()
    {
        // Arrange
        var request = new HttpRequestModel {Body = string.Empty};
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions);

        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(conditions.Body);
    }

    [TestMethod]
    public async Task BodyHandler_HandleStubGenerationAsync_HappyFlow()
    {
        // Arrange
        const string body = "POSTED!";
        var request = new HttpRequestModel {Body = body};
        var conditions = new StubConditionsModel();

        // Act
        var result = await _handler.HandleStubGenerationAsync(request, conditions);

        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual(body, ((StubConditionStringCheckingModel)conditions.Body.Single()).StringEquals);
    }
}
