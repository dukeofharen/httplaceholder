using System;
using System.Linq;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttPlaceholder.Persistence.Tests.Implementations;

[TestClass]
public class ScenarioStateStoreFacts
{
    private readonly ScenarioStateStore _store = new();

    [TestMethod]
    public void GetScenario_ScenarioIsNull_ShouldReturnNull()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = _store.GetScenario(null);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetScenario_ScenarioNotFound_ShouldReturnNull()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = _store.GetScenario("scenario-2");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetScenario_ScenarioFound_ShouldReturnCopyOfScenario()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = _store.GetScenario(scenarioName);

        // Assert
        Assert.AreNotEqual(scenario, result);
        AssertScenarioStatesAreEqual(result, scenario);
    }

    [TestMethod]
    public void GetScenario_CaseInsensitivityCheck()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        var result = _store.GetScenario(scenarioName.ToUpper());

        // Assert
        Assert.AreNotEqual(scenario, result);
        AssertScenarioStatesAreEqual(result, scenario);
    }

    [TestMethod]
    public void AddScenario_ScenarioAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, new ScenarioStateModel()));

        // Act / Assert
        Assert.ThrowsException<InvalidOperationException>(() =>
                _store.AddScenario(scenarioName, new ScenarioStateModel()),
            "Scenario state with key 'scenario-1' already exists.");
    }

    [TestMethod]
    public void AddScenario_ShouldAddScenario()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};

        // Act
        var result = _store.AddScenario(scenarioName, scenario);

        // Assert
        Assert.AreNotEqual(scenario, result);
        AssertScenarioStatesAreEqual(result, scenario);
    }

    [TestMethod]
    public void AddScenario_CaseInsensitivityCheck()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};

        // Act
        _store.AddScenario(scenarioName.ToUpper(), scenario);

        // Assert
        Assert.AreEqual(scenarioName, _store.Scenarios.Keys.Single());
    }

    [TestMethod]
    public void UpdateScenario_ScenarioNotFound_ShouldNotUpdate()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var scenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, scenario));

        // Act
        _store.UpdateScenario("scenario-2", new ScenarioStateModel());

        // Assert
        AssertScenarioStatesAreEqual(_store.Scenarios.Values.Single(), scenario);
    }

    [TestMethod]
    public void UpdateScenario_ScenarioFound_ShouldUpdateScenario()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var existingScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, existingScenario));
        var newScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-2", HitCount = 5};

        // Act
        _store.UpdateScenario(scenarioName, newScenario);

        // Assert
        var scenarioInStore = _store.Scenarios.Values.Single();
        Assert.AreNotEqual(newScenario, scenarioInStore);
        AssertScenarioStatesAreEqual(scenarioInStore, newScenario);
    }

    [TestMethod]
    public void UpdateScenario_CaseInsensitivityCheck()
    {
        // Arrange
        const string scenarioName = "scenario-1";
        var existingScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-1", HitCount = 3};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenarioName, existingScenario));
        var newScenario = new ScenarioStateModel {Scenario = scenarioName, State = "state-2", HitCount = 5};

        // Act
        _store.UpdateScenario(scenarioName.ToUpper(), newScenario);

        // Assert
        var scenarioInStore = _store.Scenarios.Values.Single();
        Assert.AreNotEqual(newScenario, scenarioInStore);
        AssertScenarioStatesAreEqual(scenarioInStore, newScenario);
    }

    [TestMethod]
    public void GetScenarioLock_ShouldGetLock()
    {
        // Arrange
        const string scenario1 = "scenario-1";
        const string scenario2 = "scenario-2";
        var lock1 = new object();
        Assert.IsTrue(_store.ScenarioLocks.TryAdd(scenario1, lock1));

        // Act
        var result1 = _store.GetScenarioLock(scenario1);
        var result2 = _store.GetScenarioLock(scenario2);

        // Assert
        Assert.AreEqual(lock1, result1);
        Assert.AreNotEqual(lock1, result2);
        Assert.AreEqual(2, _store.ScenarioLocks.Count);
    }

    [TestMethod]
    public void GetScenarioLock_CaseInsensitivityCheck()
    {
        // Arrange
        const string scenario = "scenario-1";
        var scenarioLock = new object();
        Assert.IsTrue(_store.ScenarioLocks.TryAdd(scenario, scenarioLock));

        // Act
        var result1 = _store.GetScenarioLock(scenario);
        var result2 = _store.GetScenarioLock(scenario.ToUpper());

        // Assert
        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void GetAllScenarios_HappyFlow()
    {
        // Arrange
        var scenario1 = new ScenarioStateModel {Scenario = "scenario1", State = "state1", HitCount = 2};
        var scenario2 = new ScenarioStateModel {Scenario = "scenario2", State = "state2", HitCount = 3};
        Assert.IsTrue(_store.Scenarios.TryAdd(scenario1.Scenario, scenario1));
        Assert.IsTrue(_store.Scenarios.TryAdd(scenario2.Scenario, scenario2));

        // Act
        var result = _store.GetAllScenarios().ToArray();

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
        const string scenario = "scenario-1";
        Assert.IsTrue(_store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(_store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = _store.DeleteScenario(null);

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(_store.Scenarios.Any());
        Assert.IsTrue(_store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteScenario_ScenarioFound_ShouldReturnTrue()
    {
        // Arrange
        const string scenario = "scenario-1";
        Assert.IsTrue(_store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(_store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = _store.DeleteScenario(scenario);

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(_store.Scenarios.Any());
        Assert.IsFalse(_store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteScenario_ScenarioNotFound_ShouldReturnFalse()
    {
        // Arrange
        const string scenario = "scenario-1";
        Assert.IsTrue(_store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(_store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = _store.DeleteScenario("scenario-2");

        // Assert
        Assert.IsFalse(result);
        Assert.IsTrue(_store.Scenarios.Any());
        Assert.IsTrue(_store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteScenario_CaseInsensitivityCheck()
    {
        // Arrange
        const string scenario = "scenario-1";
        Assert.IsTrue(_store.Scenarios.TryAdd(scenario, new ScenarioStateModel()));
        Assert.IsTrue(_store.ScenarioLocks.TryAdd(scenario, new object()));

        // Act
        var result = _store.DeleteScenario(scenario.ToUpper());

        // Assert
        Assert.IsTrue(result);
        Assert.IsFalse(_store.Scenarios.Any());
        Assert.IsFalse(_store.ScenarioLocks.Any());
    }

    [TestMethod]
    public void DeleteAllScenarios_HappyFlow()
    {
        // Arrange
        Assert.IsTrue(_store.Scenarios.TryAdd("scenario-1", new ScenarioStateModel()));
        Assert.IsTrue(_store.ScenarioLocks.TryAdd("scenario-1", new object()));

        // Act
        _store.DeleteAllScenarios();

        // Assert
        Assert.IsFalse(_store.Scenarios.Any());
        Assert.IsFalse(_store.ScenarioLocks.Any());
    }

    private static void AssertScenarioStatesAreEqual(ScenarioStateModel actual, ScenarioStateModel expected)
    {
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected.Scenario, actual.Scenario);
        Assert.AreEqual(expected.State, actual.State);
        Assert.AreEqual(expected.HitCount, actual.HitCount);
    }
}