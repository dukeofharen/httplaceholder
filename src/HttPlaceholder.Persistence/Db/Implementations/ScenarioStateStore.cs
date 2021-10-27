using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    /// <inheritdoc />
    internal class ScenarioStateStore : IScenarioStateStore
    {
        /// <inheritdoc />
        public ScenarioStateModel GetScenario(string scenario) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public ScenarioStateModel AddScenario(string scenario, ScenarioStateModel scenarioStateModel) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public void UpdateScenario(string scenario, ScenarioStateModel scenarioStateModel) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public object GetScenarioLock(string scenario) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public void DeleteScenario(string scenario) => throw new System.NotImplementedException();
    }
}
