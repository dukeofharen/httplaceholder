using System.Threading;
using HttPlaceholder.Application.Scenarios.Commands.SetScenario;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.Scenarios.Commands
{
    [TestClass]
    public class SetScenarioCommandHandlerFacts
    {
        private readonly AutoMocker _mocker = new();

        [TestCleanup]
        public void Cleanup() => _mocker.VerifyAll();

        [TestMethod]
        public void Handle_HappyFlow()
        {
            // Arrange
            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
            var handler = _mocker.CreateInstance<SetScenarioCommandHandler>();
            var request = new SetScenarioCommand(new ScenarioStateModel(), "scenario-1");

            // Act
            handler.Handle(request, CancellationToken.None);

            // Assert
            scenarioServiceMock.Verify(m => m.SetScenario(request.ScenarioName, request.ScenarioStateModel));
        }
    }
}
