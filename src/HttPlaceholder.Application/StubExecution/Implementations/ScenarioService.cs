using System.Collections.Generic;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    /// <inheritdoc />
    internal class ScenarioService : IScenarioService
    {
        private readonly IScenarioStateStore _scenarioStateStore;

        public ScenarioService(IScenarioStateStore scenarioStateStore)
        {
            _scenarioStateStore = scenarioStateStore;
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

        private ScenarioStateModel GetOrAddScenarioState(string scenario) =>
            _scenarioStateStore.GetScenario(scenario) ??
            _scenarioStateStore.AddScenario(
                scenario,
                new ScenarioStateModel(scenario));
    }
}
