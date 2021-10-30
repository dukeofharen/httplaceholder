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

        private ScenarioStateModel GetOrAddScenarioState(string scenario) =>
            _scenarioStateStore.GetScenario(scenario) ??
            _scenarioStateStore.AddScenario(
                scenario,
                new ScenarioStateModel(scenario));
    }
}
