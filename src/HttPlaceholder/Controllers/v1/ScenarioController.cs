﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Scenarios.Commands;
using HttPlaceholder.Application.Scenarios.Queries;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Web.Shared.Authorization;
using HttPlaceholder.Web.Shared.Dto.v1.Scenarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     Controller for the scenarios.
/// </summary>
[Route("ph-api/scenarios")]
[ApiAuthorization]
public class ScenarioController : BaseApiController
{
    /// <summary>
    ///     Gets all scenarios.
    /// </summary>
    /// <returns>OK, with all scenarios.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ScenarioStateDto>>> GetAllScenarioStates() =>
        Ok(Map<IEnumerable<ScenarioStateDto>>(await Send(new GetAllScenariosQuery())));

    /// <summary>
    ///     Gets a specific scenario.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <returns>The <see cref="ScenarioStateDto" />.</returns>
    [HttpGet("{scenario}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScenarioStateDto>> GetScenario([FromRoute] string scenario) =>
        Ok(Map<ScenarioStateDto>(await Send(new GetScenarioQuery(scenario))));

    /// <summary>
    ///     Sets the scenario state to a new value.
    /// </summary>
    /// <param name="scenarioState">The new scenario state.</param>
    /// <param name="scenario">The scenario name.</param>
    /// <returns>No content.</returns>
    [HttpPut("{scenario}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> SetScenario([FromBody] ScenarioStateInputDto scenarioState,
        [FromRoute] string scenario)
    {
        var input = MapAndSet<ScenarioStateModel>(scenarioState, s => s.Scenario = scenario);
        await Send(new SetScenarioCommand(input, scenario));
        return NoContent();
    }

    /// <summary>
    ///     Deletes / clears a scenario.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{scenario}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteScenario([FromRoute] string scenario)
    {
        await Send(new DeleteScenarioCommand(scenario));
        return NoContent();
    }

    /// <summary>
    ///     Deletes all scenarios.
    /// </summary>
    /// <returns>No content.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAllScenarios()
    {
        await Send(new DeleteAllScenariosCommand());
        return NoContent();
    }
}
