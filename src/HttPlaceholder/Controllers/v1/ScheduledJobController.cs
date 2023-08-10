using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.ScheduledJobs.Commands.ExecuteScheduledJob;
using HttPlaceholder.Application.ScheduledJobs.Queries.GetScheduledJobNames;
using HttPlaceholder.Web.Shared.Authorization;
using HttPlaceholder.Web.Shared.Dto.v1.ScheduledJobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     A controller that is used for manually calling a scheduled job.
/// </summary>
[Route("ph-api/scheduledJob")]
[ApiAuthorization]
public class ScheduledJobController : BaseApiController
{
    /// <summary>
    ///     Runs a specified scheduled job.
    /// </summary>
    /// <param name="jobName">The name of the scheduled job to run.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK.</returns>
    [HttpPost("{jobName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RunScheduledJob([FromRoute] string jobName, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new ExecuteScheduledJobCommand(jobName), cancellationToken);
        var statusCode = result.Failed ? HttpStatusCode.InternalServerError : HttpStatusCode.OK;

        return StatusCode((int)statusCode, new JobExecutionResultDto(result.Message));
    }

    /// <summary>
    ///     An endpoint for retrieving all the scheduled job names that can can be executed.
    /// </summary>
    /// <returns>A list of scheduled job names.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetScheduledJobNames() =>
        Ok(await Mediator.Send(new GetScheduledJobNamesQuery()));
}
