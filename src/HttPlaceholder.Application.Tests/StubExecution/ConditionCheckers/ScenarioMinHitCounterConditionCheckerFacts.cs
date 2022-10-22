using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class ScenarioMinHitCounterConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_MinHitsConditionNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub(null, "min-hits");
        var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ActualHitCountIsNull_ShouldReturnInvalid()
    {
        // Arrange
        var stub = CreateStub(1, "min-hits");
        var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

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
        var stub = CreateStub(3, "min-hits");
        var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCountAsync(stub.Scenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("Scenario 'min-hits' should have at least '3' hits, but only '2' hits were counted.",
            result.Log);
    }

    [TestMethod]
    public async Task ValidateAsync_HitCountIsMet_ShouldReturnValid()
    {
        // Arrange
        var stub = CreateStub(3, "min-hits");
        var checker = _mocker.CreateInstance<ScenarioMinHitCounterConditionChecker>();

        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetHitCountAsync(stub.Scenario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    private static StubModel CreateStub(int? minHits, string scenario) =>
        new()
        {
            Scenario = scenario,
            Conditions = new StubConditionsModel {Scenario = new StubConditionScenarioModel {MinHits = minHits}}
        };
}
