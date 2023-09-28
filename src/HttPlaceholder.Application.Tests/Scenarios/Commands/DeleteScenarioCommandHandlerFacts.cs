using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Scenarios.Commands.DeleteScenario;
using HttPlaceholder.Application.StubExecution;

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
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<DeleteScenarioCommandHandler>();

        const string scenarioName = "scenario-1";
        stubContextMock
            .Setup(m => m.DeleteScenarioAsync(scenarioName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act / Assert
        await Assert.ThrowsExceptionAsync<NotFoundException>(() =>
            handler.Handle(new DeleteScenarioCommand(scenarioName), CancellationToken.None));
    }

    [TestMethod]
    public async Task Handle_ScenarioFound_ShouldDeleteScenario()
    {
        // Arrange
        var stubContextMock = _mocker.GetMock<IStubContext>();
        var handler = _mocker.CreateInstance<DeleteScenarioCommandHandler>();

        const string scenarioName = "scenario-1";
        stubContextMock
            .Setup(m => m.DeleteScenarioAsync(scenarioName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await handler.Handle(new DeleteScenarioCommand(scenarioName), CancellationToken.None);

        // Assert
        stubContextMock.Verify(m => m.DeleteScenarioAsync(scenarioName, It.IsAny<CancellationToken>()));
    }
}
