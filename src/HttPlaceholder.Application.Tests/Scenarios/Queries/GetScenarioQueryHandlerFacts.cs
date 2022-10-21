using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Scenarios.Queries.GetScenario;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

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
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        var handler = _mocker.CreateInstance<GetScenarioQueryHandler>();

        const string scenarioName = "scenario-1";
        scenarioServiceMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns((ScenarioStateModel)null);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
            handler.Handle(new GetScenarioQuery(scenarioName), CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_ScenarioFound_ShouldReturnScenario()
    {
        // Arrange
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        var handler = _mocker.CreateInstance<GetScenarioQueryHandler>();

        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel(scenarioName);
        scenarioServiceMock
            .Setup(m => m.GetScenario(scenarioName))
            .Returns(scenario);

        // Act
        var result = await handler.Handle(new GetScenarioQuery(scenarioName), CancellationToken.None);

        // Assert
        Assert.AreEqual(scenario, result);
    }
}
