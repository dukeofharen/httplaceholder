using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Persistence.Implementations;

internal class ScenarioStateStore : IScenarioStateStore, ISingletonService
{
    private readonly IHttpContextService _httpContextService;
    internal readonly ConcurrentDictionary<string, object> ScenarioLocks = new();
    internal readonly ConcurrentDictionary<string, ScenarioStateModel> Scenarios = new();

    public ScenarioStateStore(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public ScenarioStateModel GetScenario(string scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return null;
        }

        var lookupKey = scenario.ToLower();
        return !Scenarios.ContainsKey(lookupKey) ? null : CopyScenarioStateModel(Scenarios[lookupKey]);
    }

    /// <inheritdoc />
    public ScenarioStateModel AddScenario(string scenario, ScenarioStateModel scenarioStateModel)
    {
        var lookupKey = scenario.ToLower();
        var scenarioToAdd = CopyScenarioStateModel(scenarioStateModel);
        if (!Scenarios.TryAdd(lookupKey, scenarioToAdd))
        {
            throw new InvalidOperationException($"Scenario state with key '{lookupKey}' already exists.");
        }

        _httpContextService.SetItem("scenarioState", CopyScenarioStateModel(scenarioStateModel));
        return scenarioToAdd;
    }

    /// <inheritdoc />
    public void UpdateScenario(string scenario, ScenarioStateModel scenarioStateModel)
    {
        var lookupKey = scenario.ToLower();
        if (!Scenarios.ContainsKey(lookupKey))
        {
            return;
        }

        var existingScenarioState = Scenarios[lookupKey];
        var newScenarioState = CopyScenarioStateModel(scenarioStateModel);
        if (!Scenarios.TryUpdate(lookupKey, newScenarioState, existingScenarioState))
        {
            throw new InvalidOperationException(
                $"Something went wrong with updating scenario with key '{lookupKey}'.");
        }

        _httpContextService.SetItem("scenarioState", CopyScenarioStateModel(scenarioStateModel));
    }

    /// <inheritdoc />
    public object GetScenarioLock(string scenario)
    {
        var lookupKey = scenario.ToLower();
        if (ScenarioLocks.TryGetValue(lookupKey, out var foundLock))
        {
            return foundLock;
        }

        var scenarioLock = new object();
        if (!ScenarioLocks.TryAdd(lookupKey, scenarioLock))
        {
            throw new InvalidOperationException($"Could not add scenario lock for scenario '{lookupKey}'.");
        }

        return scenarioLock;
    }

    /// <inheritdoc />
    public IEnumerable<ScenarioStateModel> GetAllScenarios() => Scenarios.Values.Select(CopyScenarioStateModel);

    /// <inheritdoc />
    public bool DeleteScenario(string scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return false;
        }

        var lookupKey = scenario.ToLower();
        ScenarioLocks.TryRemove(lookupKey, out _);
        return Scenarios.TryRemove(lookupKey, out _);
    }

    /// <inheritdoc />
    public void DeleteAllScenarios()
    {
        Scenarios.Clear();
        ScenarioLocks.Clear();
    }

    private static ScenarioStateModel CopyScenarioStateModel(ScenarioStateModel input) => new()
    {
        Scenario = input.Scenario, State = input.State, HitCount = input.HitCount
    };
}
