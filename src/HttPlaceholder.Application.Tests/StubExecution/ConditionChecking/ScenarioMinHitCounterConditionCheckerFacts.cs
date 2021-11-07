﻿using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionChecking
{
    [TestClass]
    public class ScenarioMinHitCounterConditionCheckerFacts
    {
        private readonly AutoMocker _mocker = new();

        [TestCleanup]
        public void Cleanup() => _mocker.VerifyAll();

        [TestMethod]
        public void Validate_MinHitsConditionNotSet_ShouldReturnNotExecuted()
        {
            // Arrange
            var stub = CreateStub(null, "min-hits");
            var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

            // Act
            var result = checker.Validate(stub);

            // Assert
            Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
        }

        [TestMethod]
        public void Validate_ActualHitCountIsNull_ShouldReturnInvalid()
        {
            // Arrange
            var stub = CreateStub(1, "min-hits");
            var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
            scenarioServiceMock
                .Setup(m => m.GetHitCount(stub.Scenario))
                .Returns((int?)null);

            // Act
            var result = checker.Validate(stub);

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
            Assert.AreEqual("No hit count could be found.", result.Log);
        }

        [TestMethod]
        public void Validate_HitCountIsNotMet_ShouldReturnInvalid()
        {
            // Arrange
            var stub = CreateStub(3, "min-hits");
            var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
            scenarioServiceMock
                .Setup(m => m.GetHitCount(stub.Scenario))
                .Returns(1);

            // Act
            var result = checker.Validate(stub);

            // Assert
            Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
            Assert.AreEqual("Scenario 'min-hits' should have at least '3' hits, but only '2' hits were counted.", result.Log);
        }

        [TestMethod]
        public void Validate_HitCountIsMet_ShouldReturnValid()
        {
            // Arrange
            var stub = CreateStub(3, "min-hits");
            var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

            var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
            scenarioServiceMock
                .Setup(m => m.GetHitCount(stub.Scenario))
                .Returns(2);

            // Act
            var result = checker.Validate(stub);

            // Assert
            Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        }

        private static StubModel CreateStub(int? minHits, string scenario) =>
            new StubModel
            {
                Scenario = scenario,
                Conditions = new StubConditionsModel {Scenario = new StubConditionScenarioModel {MinHits = minHits}}
            };
    }
}
