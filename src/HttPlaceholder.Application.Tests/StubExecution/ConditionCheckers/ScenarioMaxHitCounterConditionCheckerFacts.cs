using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class ScenarioMaxHitCounterConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_MaxHitsConditionNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub(null, "max-hits");
        var checker = _mocker.CreateInstance<ScenarioMaxHitCounterConditionChecker>();

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ActualHitCountIsNull_ShouldReturnInvalid()
    {
        // Arrange
        var stub = CreateStub(1, "max-hits");
        var checker = _mocker.CreateInstance<ScenarioMaxHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCountAsync(stub.Scenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync((int?)null);

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("No hit count could be found.", result.Log);
    }

    [TestMethod]
    public async Task ValidateAsync_HitCountIsNotMet_ShouldReturnInvalid()
    {
        // Arrange
        var stub = CreateStub(3, "max-hits");
        var checker = _mocker.CreateInstance<ScenarioMaxHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCountAsync(stub.Scenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("Scenario 'max-hits' should have less than '3' hits, but '3' hits were counted.", result.Log);
    }

    [TestMethod]
    public async Task ValidateAsync_HitCountIsMet_ShouldReturnValid()
    {
        // Arrange
        var stub = CreateStub(3, "max-hits");
        var checker = _mocker.CreateInstance<ScenarioMaxHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCountAsync(stub.Scenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    private static StubModel CreateStub(int? maxHits, string scenario) =>
        new()
        {
            Scenario = scenario,
            Conditions = new StubConditionsModel {Scenario = new StubConditionScenarioModel {MaxHits = maxHits}}
        };
}
