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
                var scenarioState = _scenarioStateStore.GetScenario(scenario);
                scenarioState.HitCount++;
                _scenarioStateStore.UpdateScenario(scenario, scenarioState);
            }
        }
    }
}
