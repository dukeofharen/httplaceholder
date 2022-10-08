using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Scenarios.Commands.DeleteAllScenarios;
using HttPlaceholder.Application.Scenarios.Commands.DeleteScenario;
using HttPlaceholder.Application.Scenarios.Commands.SetScenario;
using HttPlaceholder.Application.Scenarios.Queries.GetAllScenarios;
using HttPlaceholder.Application.Scenarios.Queries.GetScenario;
using HttPlaceholder.Authorization;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Dto.v1.Scenarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// Controller for the scenarios.
/// </summary>
[Route("ph-api/scenarios")]
[ApiAuthorization]
public class ScenarioController : BaseApiController
{
    /// <summary>
    /// Gets all scenarios.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, with all scenarios.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ScenarioStateDto>>> GetAllScenarioStates(CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<ScenarioStateDto>>(await Mediator.Send(new GetAllScenariosQuery())));

    /// <summary>
    /// Gets a specific scenario.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="ScenarioStateDto"/>.</returns>
    [HttpGet("{scenario}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScenarioStateDto>> GetScenario([FromRoute] string scenario, CancellationToken cancellationToken) =>
        Ok(Mapper.Map<ScenarioStateDto>(await Mediator.Send(new GetScenarioQuery(scenario), cancellationToken)));

    /// <summary>
    /// Sets the scenario state to a new value.
    /// </summary>
    /// <param name="scenarioState">The new scenario state.</param>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpPut("{scenario}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> SetScenario([FromBody] ScenarioStateInputDto scenarioState,
        [FromRoute] string scenario,
        CancellationToken cancellationToken)
    {
        var input = Mapper.MapAndSet<ScenarioStateModel>(scenarioState, _ => _.Scenario = scenario);
        await Mediator.Send(new SetScenarioCommand(input, scenario), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes / clears a scenario.
    /// </summary>
    /// <param name="scenario">The scenario name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete("{scenario}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteScenario([FromRoute] string scenario, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteScenarioCommand(scenario), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes all scenarios.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAllScenarios(CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteAllScenariosCommand(), cancellationToken);
        return NoContent();
    }
}
