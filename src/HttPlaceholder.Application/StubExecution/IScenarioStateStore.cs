using System;
using System.Collections.Generic;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Defines a class that is used to keep track of the different scenario states.
/// </summary>
public interface IScenarioStateStore
{
    /// <summary>
    ///     Retrieves a scenario. When the scenario has not been found, null will be returned.
    /// </summary>
    /// <param name="scenario">The scenario name. The scenario name is case insensitive.</param>
    /// <returns>The <see cref="ScenarioStateModel" /> or null.</returns>
    ScenarioStateModel GetScenario(string scenario);

    /// <summary>
    ///     Adds a scenario. Throws <see cref="InvalidOperationException" /> if scenario is already present.
    /// </summary>
    /// <param name="scenario">The scenario name. The scenario name is case insensitive.</param>
    /// <param name="scenarioStateModel">The scenario to add.</param>
    /// <returns>The added <see cref="ScenarioStateModel" />.</returns>
    ScenarioStateModel AddScenario(string scenario, ScenarioStateModel scenarioStateModel);

    /// <summary>
    ///     Updates a scenario.
    /// </summary>
    /// <param name="scenario">The scenario name. The scenario name is case insensitive.</param>
    /// <param name="scenarioStateModel">The new scenario contents.</param>
    void UpdateScenario(string scenario, ScenarioStateModel scenarioStateModel);

    /// <summary>
    ///     Retrieves a lock specifically for a scenario.
    ///     Locking in this case is done to prevent race conditions when updating scenarios.
    /// </summary>
    /// <param name="scenario">The scenario name. The scenario name is case insensitive.</param>
    /// <returns>A scenario lock.</returns>
    object GetScenarioLock(string scenario);

    /// <summary>
    ///     Retrieves a list of all scenarios.
    /// </summary>
    /// <returns>A list of <see cref="ScenarioStateModel" />.</returns>
    IEnumerable<ScenarioStateModel> GetAllScenarios();

    /// <summary>
    ///     Deletes a given scenario.
    /// </summary>
    /// <param name="scenario">The scenario name. The scenario name is case insensitive.</param>
    /// <returns>True if scenario was found and deleted; false otherwise.</returns>
    bool DeleteScenario(string scenario);

    /// <summary>
    ///     Deletes all scenarios.
    /// </summary>
    void DeleteAllScenarios();
}
