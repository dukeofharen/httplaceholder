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
    }
}
