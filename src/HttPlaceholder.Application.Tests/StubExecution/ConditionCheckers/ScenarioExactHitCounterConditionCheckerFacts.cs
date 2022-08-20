using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class ScenarioExactHitCounterConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_ExactHitsConditionNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub(null, "exact-hits");
        var checker = _mocker.CreateInstance<ScenarioExactHitCounterConditionChecker>();

        // Act
        var result = await checker.ValidateAsync(stub);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ActualHitCountIsNull_ShouldReturnInvalid()
    {
        // Arrange
        var stub = CreateStub(1, "exact-hits");
        var checker = _mocker.CreateInstance<ScenarioExactHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCount(stub.Scenario))
            .Returns((int?)null);

        // Act
        var result = await checker.ValidateAsync(stub);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("No hit count could be found.", result.Log);
    }

    [TestMethod]
    public async Task ValidateAsync_HitCountIsNotMet_ShouldReturnInvalid()
    {
        // Arrange
        var stub = CreateStub(3, "exact-hits");
        var checker = _mocker.CreateInstance<ScenarioExactHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCount(stub.Scenario))
            .Returns(3);

        // Act
        var result = await checker.ValidateAsync(stub);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("Scenario 'exact-hits' should have exactly '3' hits, but '4' hits were counted.", result.Log);

    }

    [TestMethod]
    public async Task ValidateAsync_HitCountIsMet_ShouldReturnValid()
    {
        // Arrange
        var stub = CreateStub(3, "exact-hits");
        var checker = _mocker.CreateInstance<ScenarioExactHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCount(stub.Scenario))
            .Returns(2);

        // Act
        var result = await checker.ValidateAsync(stub);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    private static StubModel CreateStub(int? exactHits, string scenario) =>
        new()
        {
            Scenario = scenario,
            Conditions = new StubConditionsModel {Scenario = new StubConditionScenarioModel {ExactHits = exactHits}}
        };
}
