using System.Collections.Generic;
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
    /// <param name="fromIdentifier">
    ///     The identifier from which to find items. If this is not set; means to query from the
    ///     start.
    /// </param>
    /// <param name="itemsPerPage">The number of items to show on a page.</param>
    /// <returns>All request results</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetAll(
        [FromHeader(Name = "x-from-identifier")]
        string fromIdentifier,
        [FromHeader(Name = "x-items-per-page")]
        int? itemsPerPage) =>
        Ok(Map<IEnumerable<RequestResultDto>>(await Send(
            new GetAllRequestsQuery(fromIdentifier, itemsPerPage))));

    /// <summary>
    ///     Get overview of all Requests.
    /// </summary>
    /// <param name="fromIdentifier">
    ///     The identifier from which to find items. If this is not set; means to query from the
    ///     start.
    /// </param>
    /// <param name="itemsPerPage">The number of items to show on a page.</param>
    /// <returns>All request results</returns>
    [HttpGet("overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestOverviewDto>>> GetOverview(
        [FromHeader(Name = "x-from-identifier")]
        string fromIdentifier,
        [FromHeader(Name = "x-items-per-page")]
        int? itemsPerPage) =>
        Ok(Map<IEnumerable<RequestOverviewDto>>(await Send(
            new GetRequestsOverviewQuery(fromIdentifier, itemsPerPage))));

    /// <summary>
    ///     Gets a specific request by correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <returns>The request.</returns>
    [HttpGet("{correlationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestResultDto>> GetRequest([FromRoute] string correlationId) =>
        Ok(Map<RequestResultDto>(
            await Send(new GetRequestQuery(correlationId))));

    /// <summary>
    ///     Gets a specific response by request correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <returns>The request.</returns>
    [HttpGet("{correlationId}/response")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResponseDto>> GetResponse([FromRoute] string correlationId) =>
        Ok(Map<ResponseDto>(
            await Send(new GetResponseQuery(correlationId))));

    /// <summary>
    ///     Delete all requests. This call flushes all the requests.
    /// </summary>
    /// <returns>OK, but no content returned</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAll()
    {
        await Send(new DeleteAllRequestsCommand());
        return NoContent();
    }

    /// <summary>
    ///     Delete a specific request.
    /// </summary>
    /// <param name="correlationId">The correlation ID of the request to delete.</param>
    /// <returns>OK, but no content returned</returns>
    [HttpDelete("{correlationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteRequest(string correlationId) =>
        await Send(new DeleteRequestCommand(correlationId)) ? NoContent() : NotFound();

    /// <summary>
    ///     An endpoint which accepts the correlation ID of a request made earlier.
    ///     HttPlaceholder will create a stub based on this request for you to tweak later on.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="input">The input.</param>
    /// <returns>OK, with the generated stub</returns>
    [HttpPost("{correlationId}/stubs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FullStubDto>> CreateStubForRequest(
        [FromRoute] string correlationId,
        [FromBody] CreateStubForRequestInputDto input) =>
        Ok(Map<FullStubDto>(
            await Send(new CreateStubForRequestCommand(correlationId, input?.DoNotCreateStub ?? false))));
}
