using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Scenarios.Queries;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.Tests.Scenarios.Queries;

[TestClass]
public class GetScenarioQueryHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_ScenarioNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<GetScenarioQueryHandler>();

        const string scenarioName = "scenario-1";
        stubContextMock
            .Setup(m => m.GetScenarioAsync(scenarioName, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ScenarioStateModel)null);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
            handler.Handle(new GetScenarioQuery(scenarioName), CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_ScenarioFound_ShouldReturnScenario()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<GetScenarioQueryHandler>();

        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel(scenarioName);
        stubContextMock
            .Setup(m => m.GetScenarioAsync(scenarioName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(scenario);

        // Act
        var result = await handler.Handle(new GetScenarioQuery(scenarioName), CancellationToken.None);

        // Assert
        Assert.AreEqual(scenario, result);
    }
}
