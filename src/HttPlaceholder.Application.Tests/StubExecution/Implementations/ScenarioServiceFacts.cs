﻿using HttPlaceholder.Application.StubExecution;
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
        public void IncreaseHitCount_ScenarioIsNotEmpty_ScenarioIsFound_ShouldIncreaseHitCount()
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

        [TestMethod]
        public void IncreaseHitCount_ScenarioIsNotEmpty_ScenarioIsNotFound_ShouldCreateScenarioAndIncreaseHitCount()
        {
            // Arrange
            var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
            var service = _mocker.CreateInstance<ScenarioService>();

            const string scenario = "SCENARIO-1";

            scenarioStateStoreMock
                .Setup(m => m.GetScenarioLock(scenario))
                .Returns(new object());

            scenarioStateStoreMock
                .Setup(m => m.GetScenario(scenario))
                .Returns((ScenarioStateModel)null);

            ScenarioStateModel capturedCreatedScenarioStateModel = null;
            scenarioStateStoreMock
                .Setup(m => m.AddScenario(scenario, It.IsAny<ScenarioStateModel>()))
                .Callback<string, ScenarioStateModel>((_, ssm) => capturedCreatedScenarioStateModel = ssm)
                .Returns<string, ScenarioStateModel>((_, ssm) => ssm);

            ScenarioStateModel capturedScenarioStateModel = null;
            scenarioStateStoreMock
                .Setup(m => m.UpdateScenario(scenario, It.IsAny<ScenarioStateModel>()))
                .Callback<string, ScenarioStateModel>((_, ssm) => capturedScenarioStateModel = ssm);

            // Act
            service.IncreaseHitCount(scenario);

            // Assert
            Assert.IsNotNull(capturedCreatedScenarioStateModel);
            Assert.IsNotNull(capturedScenarioStateModel);
            Assert.AreEqual(1, capturedCreatedScenarioStateModel.HitCount);
            Assert.AreEqual("Start", capturedCreatedScenarioStateModel.State);
            Assert.AreEqual(scenario.ToLower(), capturedCreatedScenarioStateModel.Scenario);
            Assert.AreEqual(1, capturedScenarioStateModel.HitCount);
            Assert.AreEqual("Start", capturedScenarioStateModel.State);
            Assert.AreEqual(scenario.ToLower(), capturedScenarioStateModel.Scenario);
        }

        [TestMethod]
        public void GetHitCount_ScenarioNotSet_ShouldReturnNull()
        {
            // Arrange
            var service = _mocker.CreateInstance<ScenarioService>();

            // Act
            var result = service.GetHitCount(string.Empty);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetHitCount_ScenarioSet_ShouldReturnHitCount()
        {
            // Arrange
            var scenarioStateStoreMock = _mocker.GetMock<IScenarioStateStore>();
            var service = _mocker.CreateInstance<ScenarioService>();
            const string scenario = "SCENARIO-1";

            scenarioStateStoreMock
                .Setup(m => m.GetScenarioLock(scenario))
                .Returns(new object());

            var scenarioState = new ScenarioStateModel {Scenario = scenario, HitCount = 2};
            scenarioStateStoreMock
                .Setup(m => m.GetScenario(scenario))
                .Returns(scenarioState);

            // Act
            var result = service.GetHitCount(scenario);

            // Assert
            Assert.AreEqual(2, result);
        }
    }
}
