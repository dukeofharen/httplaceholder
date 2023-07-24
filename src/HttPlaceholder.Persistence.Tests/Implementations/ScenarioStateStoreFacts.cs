using System.Linq;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Implementations;

namespace HttPlaceholder.Persistence.Tests.Implementations;

[TestClass]
public class ScenarioStateStoreFacts
{
    private readonly AutoMocker _mocker = new();

    [TestMethod]
    public void GetScenario_ScenarioIsNull_ShouldReturnNull()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName};
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = store.GetScenario(null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetScenario_ScenarioNotFound_ShouldReturnNull()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName};
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = store.GetScenario("scenario-2");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetScenario_ScenarioFound_ShouldReturnCopyOfScenario()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = store.GetScenario(scenarioName);

        // Assert
        Assert.AreNotEqual(scenario, result);
        AssertScenarioStatesAreEqual(result, scenario);
    }

    [TestMethod]
    public void GetScenario_CaseInsensitivityCheck()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = store.GetScenario(scenarioName.ToUpper());

        // Assert
        Assert.AreNotEqual(scenario, result);
        AssertScenarioStatesAreEqual(result, scenario);
    }

    [TestMethod]
    public void AddScenario_ScenarioAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, new ScenarioStateModel()));

        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
                store.AddScenario(scenarioName, new ScenarioStateModel()),
            "Scenario state with key 'scenario-1' already exists.");
    }

    [TestMethod]
    public void AddScenario_ShouldAddScenario()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};

        // Act
        var result = store.AddScenario(scenarioName, scenario);

        // Assert
        Assert.AreNotEqual(scenario, result);
        AssertScenarioStatesAreEqual(result, scenario);
    }

    [TestMethod]
    public void AddScenario_CaseInsensitivityCheck()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};

        // Act
        store.AddScenario(scenarioName.ToUpper(), scenario);

        // Assert
        Assert.AreEqual(scenarioName, store.Scenarios.Keys.Single());
    }

    [TestMethod]
    public void UpdateScenario_ScenarioNotFound_ShouldNotUpdate()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        store.UpdateScenario("scenario-2", new ScenarioStateModel());

        // Assert
        AssertScenarioStatesAreEqual(store.Scenarios.Values.Single(), scenario);
    }

    [TestMethod]
    public void UpdateScenario_ScenarioFound_ShouldUpdateScenario()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var existingScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, existingScenario));
        var newScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-2", HitCount = 5};

        // Act
        store.UpdateScenario(scenarioName, newScenario);

        // Assert
        var scenarioInStore = store.Scenarios.Values.Single();
        Assert.AreNotEqual(newScenario, scenarioInStore);
        AssertScenarioStatesAreEqual(scenarioInStore, newScenario);
    }

    [TestMethod]
    public void UpdateScenario_CaseInsensitivityCheck()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenarioName = "scenario-1";
        var existingScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(store.Scenarios.TryAdd(scenarioName, existingScenario));
        var newScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-2", HitCount = 5};

        // Act
        store.UpdateScenario(scenarioName.ToUpper(), newScenario);

        // Assert
        var scenarioInStore = store.Scenarios.Values.Single();
        Assert.AreNotEqual(newScenario, scenarioInStore);
        AssertScenarioStatesAreEqual(scenarioInStore, newScenario);
    }

    [TestMethod]
    public void GetScenarioLock_ShouldGetLock()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenario1 = "scenario-1";
        const string scenario2 = "scenario-2";
        var lock1 = new object();
        Assert.IsTrue(store.ScenarioLocks.TryAdd(scenario1, lock1));

        // Act
        var result1 = store.GetScenarioLock(scenario1);
        var result2 = store.GetScenarioLock(scenario2);

        // Assert
        Assert.AreEqual(lock1, result1);
        Assert.AreNotEqual(lock1, result2);
        Assert.AreEqual(2, store.ScenarioLocks.Count);
    }

    [TestMethod]
    public void GetScenarioLock_CaseInsensitivityCheck()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenario = "scenario-1";
        var scenarioLock = new object();
        Assert.IsTrue(store.ScenarioLocks.TryAdd(scenario, scenarioLock));

        // Act
        var result1 = store.GetScenarioLock(scenario);
        var result2 = store.GetScenarioLock(scenario.ToUpper());

        // Assert
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void GetAllScenarios_HappyFlow()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        var scenario1 = new ScenarioStateModel {Scenario = "scenario1", State = "state1", HitCount = 2};
        var scenario2 = new ScenarioStateModel {Scenario = "scenario2", State = "state2", HitCount = 3};
        Assert.IsTrue(store.Scenarios.TryAdd(scenario1.Scenario, scenario1));
        Assert.IsTrue(store.Scenarios.TryAdd(scenario2.Scenario, scenario2));

        // Act
        var result = store.GetAllScenarios().ToArray();

        // Assert
        Assert.AreEqual(2, result.Length);
        var scenarioResult1 = result.Single(s => s.Scenario == scenario1.Scenario);
        var scenarioResult2 = result.Single(s => s.Scenario == scenario2.Scenario);

        Assert.AreNotEqual(scenarioResult1, scenario1);
        Assert.AreNotEqual(scenarioResult2, scenario2);
        AssertScenarioStatesAreEqual(scenarioResult1, scenario1);
        AssertScenarioStatesAreEqual(scenarioResult2, scenario2);
    }

    [TestMethod]
    public void DeleteScenario_ScenarioNotSet_ShouldNotDeleteScenario()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenario = "scenario-1";
        Assert.IsTrue(store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = store.DeleteScenario(null);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(store.Scenarios.Any());
        Assert.IsTrue(store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteScenario_ScenarioFound_ShouldReturnTrue()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenario = "scenario-1";
        Assert.IsTrue(store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = store.DeleteScenario(scenario);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(store.Scenarios.Any());
        Assert.IsFalse(store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteScenario_ScenarioNotFound_ShouldReturnFalse()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenario = "scenario-1";
        Assert.IsTrue(store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = store.DeleteScenario("scenario-2");

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(store.Scenarios.Any());
        Assert.IsTrue(store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteScenario_CaseInsensitivityCheck()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        const string scenario = "scenario-1";
        Assert.IsTrue(store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = store.DeleteScenario(scenario.ToUpper());

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(store.Scenarios.Any());
        Assert.IsFalse(store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteAllScenarios_HappyFlow()
    {
        // Arrange
        var store = _mocker.CreateInstance<ScenarioStateStore>();
        Assert.IsTrue(store.Scenarios.TryAdd("scenario-1", new ScenarioStateModel()));
        Assert.IsTrue(store.ScenarioLocks.TryAdd("scenario-1", new object()));

        // Act
        store.DeleteAllScenarios();

        // Assert
        Assert.IsFalse(store.Scenarios.Any());
        Assert.IsFalse(store.ScenarioLocks.Any());
    }

    private static void AssertScenarioStatesAreEqual(ScenarioStateModel actual, ScenarioStateModel expected)
    {
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Scenario, actual.Scenario);
        Assert.AreEqual(expected.State, actual.State);
        Assert.AreEqual(expected.HitCount, actual.HitCount);
    }
}
