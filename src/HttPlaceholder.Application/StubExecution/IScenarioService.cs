using System.Collections.Generic;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution
{
    /// <summary>
    /// Describes a class that is used for working with scenarios and scenario state.
    /// </summary>
    public interface IScenarioService
    {
        /// <summary>
        /// Increases the hit count of a specific scenario.
        /// </summary>
        /// <param name="scenario">The scenario name. Is case insensitive.</param>
        void IncreaseHitCount(string scenario);

        /// <summary>
        /// Get the hit count of a specific scenario.
        /// </summary>
        /// <param name="scenario">The scenario name. Is case insensitive.</param>
        /// <returns>The hit count.</returns>
        int? GetHitCount(string scenario);

        /// <summary>
        /// Returns all scenarios.
        /// </summary>
        /// <returns>A list of all <see cref="ScenarioStateModel"/>.</returns>
        IEnumerable<ScenarioStateModel> GetAllScenarios();

        /// <summary>
        /// Returns a specific scenario. Is case insensitive.
        /// </summary>
        /// <param name="scenario">The scenario name.</param>
        /// <returns>The <see cref="ScenarioStateModel"/> or null if the scenario was not found.</returns>
        ScenarioStateModel GetScenario(string scenario);

        /// <summary>
        /// Sets the scenario state. Adds the scenario if it does not exist yet.
        /// </summary>
        /// <param name="scenario">The scenario name.</param>
        /// <param name="scenarioState">The scenario state.</param>
        void SetScenario(string scenario, ScenarioStateModel scenarioState);

        /// <summary>
        /// Clears a scenario state.
        /// </summary>
        /// <param name="scenario">The scenario name.</param>
        /// <returns>True if scenario was found and deleted; false otherwise.</returns>
        bool DeleteScenario(string scenario);

        /// <summary>
        /// Deletes all scenarios.
        /// </summary>
        void DeleteAllScenarios();
    }
}
