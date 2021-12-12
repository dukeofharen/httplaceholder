using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Scenarios.Commands.DeleteScenario;
using HttPlaceholder.Application.StubExecution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Scenarios.Commands;

[TestClass]
public class DeleteScenarioCommandHandlerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task Handle_ScenarioNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        var handler = _mocker.CreateInstance<DeleteScenarioCommandHandler>();

        const string scenarioName = "scenario-1";
        scenarioServiceMock
            .Setup(m => m.DeleteScenario(scenarioName))
            .Returns(false);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
            handler.Handle(new DeleteScenarioCommand(scenarioName), CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_ScenarioFound_ShoulDeleteScenario()
    {
        // Arrange
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        var handler = _mocker.CreateInstance<DeleteScenarioCommandHandler>();

        const string scenarioName = "scenario-1";
        scenarioServiceMock
            .Setup(m => m.DeleteScenario(scenarioName))
            .Returns(true);

        // Act
        await handler.Handle(new DeleteScenarioCommand(scenarioName), CancellationToken.None);

        // Assert
        scenarioServiceMock.Verify(m => m.DeleteScenario(scenarioName));
    }
}