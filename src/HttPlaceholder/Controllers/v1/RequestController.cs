using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Requests.Commands;
using HttPlaceholder.Application.Requests.Queries;
using HttPlaceholder.Web.Shared.Authorization;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     The request controller
/// </summary>
[Route("ph-api/requests")]
[ApiAuthorization]
public class RequestController : BaseApiController
{
    /// <summary>
    ///     Get all Requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="fromIdentifier">
    ///     The identifier from which to find items. If this is not set; means to query from the
    ///     start.
    /// </param>
    /// <param name="itemsPerPage">The number of items to show on a page.</param>
    /// <returns>All request results</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetAll(CancellationToken cancellationToken,
        [FromHeader(Name = "x-from-identifier")]
        string fromIdentifier, [FromHeader(Name = "x-items-per-page")] int? itemsPerPage) =>
        Ok(Mapper.Map<IEnumerable<RequestResultDto>>(await Mediator.Send(
            new GetAllRequestsQuery(fromIdentifier, itemsPerPage),
            cancellationToken)));

    /// <summary>
    ///     Get overview of all Requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <param name="fromIdentifier">
    ///     The identifier from which to find items. If this is not set; means to query from the
    ///     start.
    /// </param>
    /// <param name="itemsPerPage">The number of items to show on a page.</param>
    /// <returns>All request results</returns>
    [HttpGet("overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestOverviewDto>>> GetOverview(CancellationToken cancellationToken,
        [FromHeader(Name = "x-from-identifier")]
        string fromIdentifier, [FromHeader(Name = "x-items-per-page")] int? itemsPerPage) =>
        Ok(Mapper.Map<IEnumerable<RequestOverviewDto>>(await Mediator.Send(
            new GetRequestsOverviewQuery(fromIdentifier, itemsPerPage),
            cancellationToken)));

    /// <summary>
    ///     Gets a specific request by correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request.</returns>
    [HttpGet("{correlationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResultDto>> GetRequest([FromRoute] string correlationId,
        CancellationToken cancellationToken) =>
        Ok(Mapper.Map<RequestResultDto>(
            await Mediator.Send(new GetRequestQuery(correlationId), cancellationToken)));

    /// <summary>
    ///     Gets a specific response by request correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request.</returns>
    [HttpGet("{correlationId}/response")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto>> GetResponse([FromRoute] string correlationId,
        CancellationToken cancellationToken) =>
        Ok(Mapper.Map<ResponseDto>(
            await Mediator.Send(new GetResponseQuery(correlationId), cancellationToken)));

    /// <summary>
    ///     Delete all requests. This call flushes all the requests.
    /// </summary>
    /// <returns>OK, but no content returned</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAll(CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteAllRequestsCommand(), cancellationToken);
        return NoContent();
    }

    /// <summary>
    ///     Delete a specific request.
    /// </summary>
    /// <param name="correlationId">The correlation ID of the request to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, but no content returned</returns>
    [HttpDelete("{correlationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteRequest(string correlationId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new DeleteRequestCommand(correlationId), cancellationToken);
        return result ? NoContent() : NotFound();
    }

    /// <summary>
    ///     An endpoint which accepts the correlation ID of a request made earlier.
    ///     HttPlaceholder will create a stub based on this request for you to tweak later on.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="input">The input.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, with the generated stub</returns>
    [HttpPost("{correlationId}/stubs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FullStubDto>> CreateStubForRequest(
        [FromRoute] string correlationId,
        [FromBody] CreateStubForRequestInputDto input,
        CancellationToken cancellationToken) =>
        Ok(Mapper.Map<FullStubDto>(
            await Mediator.Send(new CreateStubForRequestCommand(correlationId, input?.DoNotCreateStub ?? false),
                cancellationToken)));
}
