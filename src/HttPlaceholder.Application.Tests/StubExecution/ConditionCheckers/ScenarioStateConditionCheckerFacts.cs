using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.ConditionCheckers;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Tests.StubExecution.ConditionCheckers;

[TestClass]
public class ScenarioStateConditionCheckerFacts
{
    private readonly AutoMocker _mocker = new();

    [TestCleanup]
    public void Cleanup() => _mocker.VerifyAll();

    [TestMethod]
    public async Task ValidateAsync_ScenarioOnStubNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub(string.Empty, "state1");

        var checker = _mocker.CreateInstance<ScenarioStateConditionChecker>();

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ScenarioStateOnStubNotSet_ShouldReturnNotExecuted()
    {
        // Arrange
        var stub = CreateStub("state1", string.Empty);

        var checker = _mocker.CreateInstance<ScenarioStateConditionChecker>();

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.NotExecuted, result.ConditionValidation);
    }

    [TestMethod]
    public async Task ValidateAsync_ScenarioStateNotSet_ShouldAddScenario_InitialState_ShouldReturnValid()
    {
        // Arrange
        const string scenario = "scenario-1";
        const string state = Constants.DefaultScenarioState;
        var stub = CreateStub(scenario, state);

        var checker = _mocker.CreateInstance<ScenarioStateConditionChecker>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetScenario(scenario))
            .Returns((ScenarioStateModel)null);

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
        scenarioServiceMock.Verify(m =>
            m.SetScenarioAsync(scenario, It.IsAny<ScenarioStateModel>(), It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task ValidateAsync_ScenarioStateNotSet_ShouldAddScenario_NonInitialState_ShouldReturnInvalid()
    {
        // Arrange
        const string scenario = "scenario-1";
        const string state = "state-1";
        var stub = CreateStub(scenario, state);

        var checker = _mocker.CreateInstance<ScenarioStateConditionChecker>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetScenario(scenario))
            .Returns((ScenarioStateModel)null);

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("Scenario 'scenario-1' is in state 'Start', but 'state-1' was expected.", result.Log);
        scenarioServiceMock.Verify(m =>
            m.SetScenarioAsync(scenario, It.IsAny<ScenarioStateModel>(), It.IsAny<CancellationToken>()));
    }

    [TestMethod]
    public async Task ValidateAsync_ScenarioStateNotCorrect_ShouldReturnInvalid()
    {
        // Arrange
        const string scenario = "scenario-1";
        const string state = "state-1";
        var stub = CreateStub(scenario, state);

        var checker = _mocker.CreateInstance<ScenarioStateConditionChecker>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetScenario(scenario))
            .Returns(new ScenarioStateModel {State = "state-2"});

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Invalid, result.ConditionValidation);
        Assert.AreEqual("Scenario 'scenario-1' is in state 'state-2', but 'state-1' was expected.", result.Log);
    }

    [DataTestMethod]
    [DataRow("state-1")]
    [DataRow("STate-1")]
    [DataRow("STATE-1")]
    [DataRow(" STATE-1 ")]
    [DataRow(" STATE-1")]
    [DataRow("state-1 ")]
    public async Task ValidateAsync_ScenarioStateCorrect_ShouldReturnValid(string currentState)
    {
        // Arrange
        const string scenario = "scenario-1";
        const string state = "state-1";
        var stub = CreateStub(scenario, state);

        var checker = _mocker.CreateInstance<ScenarioStateConditionChecker>();
        var scenarioServiceMock = _mocker.GetMock<IScenarioService>();
        scenarioServiceMock
            .Setup(m => m.GetScenario(scenario))
            .Returns(new ScenarioStateModel {State = currentState});

        // Act
        var result = await checker.ValidateAsync(stub, CancellationToken.None);

        // Assert
        Assert.AreEqual(ConditionValidationType.Valid, result.ConditionValidation);
    }

    private static StubModel CreateStub(string scenario, string state) =>
        new()
        {
            Scenario = scenario,
            Conditions = new StubConditionsModel {Scenario = new StubConditionScenarioModel {ScenarioState = state}}
        };
}
