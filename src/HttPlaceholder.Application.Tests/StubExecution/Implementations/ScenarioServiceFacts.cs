using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Implementations;
using HttPlaceholder.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.Implementations
{
    [TestClass]
    public class ScenarioServiceFacts
    {
        private readonly AutoMocker _mocker = new();

        [TestCleanup]
        public void Cleanup() => _mocker.VerifyAll();

        [TestMethod]
        public void IncreaseHitCount_ScenarioIsEmpty_ShouldNotIncreaseHitCount()
        {
            // Arrange
            var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
            var service = _mocker.CreateInstance<ScenarioService>();

            // Act
            service.IncreaseHitCount(string.Empty);

            // Assert
            scenarioStateStoreMock.Verify(m => m.GetScenarioLock(It.IsAny<string>()), Times.Never());
        }

        [TestMethod]
        public void IncreaseHitCount_ScenarioIsNotEmpty_ShouldIncreaseHitCount()
        {
            // Arrange
            var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
            var service = _mocker.CreateInstance<ScenarioService>();

            var scenario = "scenario-1";

            scenarioStateStoreMock
                .Setup(m => m.GetScenarioLock(scenario))
                .Returns(new object());

            var scenarioState = new ScenarioStateModel {Scenario = scenario, HitCount = 2};
            scenarioStateStoreMock
                .Setup(m => m.GetScenario(scenario))
                .Returns(scenarioState);

            ScenarioStateModel capturedScenarioStateModel = null;
            scenarioStateStoreMock
                .Setup(m => m.UpdateScenario(scenario, It.IsAny<ScenarioStateModel>()))
                .Callback<string, ScenarioStateModel>((_, ssm) => capturedScenarioStateModel = ssm);

            // Act
            service.IncreaseHitCount(scenario);

            // Assert
            Assert.IsNotNull(capturedScenarioStateModel);
            Assert.AreEqual(3, capturedScenarioStateModel.HitCount);
        }
    }
}
