using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Requests.Commands.CreateStubForRequest;
using HttPlaceholder.Application.Requests.Commands.DeleteAllRequests;
using HttPlaceholder.Application.Requests.Commands.DeleteRequest;
using HttPlaceholder.Application.Requests.Queries.GetAllRequests;
using HttPlaceholder.Application.Requests.Queries.GetRequest;
using HttPlaceholder.Application.Requests.Queries.GetRequestsOverview;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.Requests;
using HttPlaceholder.Dto.v1.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// Controller for requests.
/// </summary>
[Route("ph-api/requests")]
[ApiAuthorization]
public class RequestController : BaseApiController
{
    /// <summary>
    /// Get all Requests.
    /// </summary>
    /// <returns>All request results</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetAll() =>
        Ok(Mapper.Map<IEnumerable<RequestResultDto>>(await Mediator.Send(new GetAllRequestsQuery())));

    /// <summary>
    /// Get overview of all Requests.
    /// </summary>
    /// <returns>All request results</returns>
    [HttpGet("overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestOverviewDto>>> GetOverview() =>
        Ok(Mapper.Map<IEnumerable<RequestOverviewDto>>(await Mediator.Send(new GetRequestsOverviewQuery())));

    /// <summary>
    /// Gets a specific request by correlation ID.
    /// </summary>
    /// <param name="correlationId">The original correlation ID.</param>
    /// <returns>The request.</returns>
    [HttpGet("{correlationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResultDto>> GetRequest([FromRoute] string correlationId) =>
        Ok(Mapper.Map<RequestResultDto>(
            await Mediator.Send(new GetRequestQuery { CorrelationId = correlationId })));

    /// <summary>
    /// Delete all requests. This call flushes all the requests.
    /// </summary>
    /// <returns>OK, but no content returned</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAll()
    {
        await Mediator.Send(new DeleteAllRequestsCommand());
        return NoContent();
    }

    /// <summary>
    /// Delete a specific request.
    /// </summary>
    /// <param name="correlationId">The ID of the request to delete.</param>
    /// <returns>OK, but no content returned</returns>
    [HttpDelete("{correlationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteRequest(string correlationId)
    {
        var result = await Mediator.Send(new DeleteRequestCommand(correlationId));
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    /// An endpoint which accepts the correlation ID of a request made earlier.
    /// HttPlaceholder will create a stub based on this request for you to tweak later on.
    /// </summary>
    /// <returns>OK, with the generated stub</returns>
    [HttpPost("{correlationId}/stubs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FullStubDto>> CreateStubForRequest(
        [FromRoute] string correlationId,
        [FromBody] CreateStubForRequestInputDto input) =>
        Ok(Mapper.Map<FullStubDto>(
            await Mediator.Send(new CreateStubForRequestCommand(correlationId, input?.DoNotCreateStub ?? false))));
}