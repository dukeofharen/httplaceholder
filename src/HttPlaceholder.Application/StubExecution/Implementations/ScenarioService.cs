using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
internal class ScenarioService : IScenarioService
{
    private readonly IScenarioStateStore _scenarioStateStore;
    private readonly IScenarioNotify _scenarioNotify;

    public ScenarioService(IScenarioStateStore scenarioStateStore, IScenarioNotify scenarioNotify)
    {
        _scenarioStateStore = scenarioStateStore;
        _scenarioNotify = scenarioNotify;
    }

    /// <inheritdoc />
    public void IncreaseHitCount(string scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return;
        }

        lock (_scenarioStateStore.GetScenarioLock(scenario))
        {
            var scenarioState = GetOrAddScenarioState(scenario);
            scenarioState.HitCount++;
            _scenarioStateStore.UpdateScenario(scenario, scenarioState);
        }
    }

    /// <inheritdoc />
    public int? GetHitCount(string scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return null;
        }

        lock (_scenarioStateStore.GetScenarioLock(scenario))
        {
            var scenarioState = GetOrAddScenarioState(scenario);
            return scenarioState.HitCount;
        }
    }

    /// <inheritdoc />
    public IEnumerable<ScenarioStateModel> GetAllScenarios() => _scenarioStateStore.GetAllScenarios();

    /// <inheritdoc />
    public ScenarioStateModel GetScenario(string scenario) => _scenarioStateStore.GetScenario(scenario);

    /// <inheritdoc />
    public async Task SetScenarioAsync(string scenario, ScenarioStateModel scenarioState)
    {
        if (string.IsNullOrWhiteSpace(scenario) || scenarioState == null)
        {
            return;
        }

        lock (_scenarioStateStore.GetScenarioLock(scenario))
        {
            var existingScenario = _scenarioStateStore.GetScenario(scenario);
            if (existingScenario == null)
            {
                if (string.IsNullOrWhiteSpace(scenarioState.State))
                {
                    scenarioState.State = Constants.DefaultScenarioState;
                }

                _scenarioStateStore.AddScenario(scenario, scenarioState);
            }
            else
            {
                if (scenarioState.HitCount == -1)
                {
                    scenarioState.HitCount = existingScenario.HitCount;
                }

                if (string.IsNullOrWhiteSpace(scenarioState.State))
                {
                    scenarioState.State = existingScenario.State;
                }

                _scenarioStateStore.UpdateScenario(scenario, scenarioState);
            }
        }

        await _scenarioNotify.ScenarioSetAsync(scenarioState);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteScenarioAsync(string scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return false;
        }

        bool result;
        lock (_scenarioStateStore.GetScenarioLock(scenario))
        {
            result = _scenarioStateStore.DeleteScenario(scenario);
        }

        await _scenarioNotify.ScenarioDeletedAsync(scenario);
        return result;
    }

    /// <inheritdoc />
    public async Task DeleteAllScenariosAsync()
    {
        _scenarioStateStore.DeleteAllScenarios();
        await _scenarioNotify.AllScenariosDeletedAsync();
    }

    private ScenarioStateModel GetOrAddScenarioState(string scenario) =>
        _scenarioStateStore.GetScenario(scenario) ??
        _scenarioStateStore.AddScenario(
            scenario,
            new ScenarioStateModel(scenario));
}
