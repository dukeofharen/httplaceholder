using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Requests.Queries.GetByStubId;
using HttPlaceholder.Application.Stubs.Commands.AddStub;
using HttPlaceholder.Application.Stubs.Commands.AddStubs;
using HttPlaceholder.Application.Stubs.Commands.DeleteAllStubs;
using HttPlaceholder.Application.Stubs.Commands.DeleteStub;
using HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand;
using HttPlaceholder.Application.Stubs.Queries.GetAllStubs;
using HttPlaceholder.Application.Stubs.Queries.GetStub;
using HttPlaceholder.Application.Stubs.Queries.GetStubsOverview;
using HttPlaceholder.Authorization;
using HttPlaceholder.Domain;
using HttPlaceholder.Dto.v1.Requests;
using HttPlaceholder.Dto.v1.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// The stub controller.
/// </summary>
[Route("ph-api/stubs")]
[ApiAuthorization]
public class StubController : BaseApiController
{
    /// <summary>
    /// Adds a new stub.
    /// </summary>
    /// <param name="stub">The posted stub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, with the created stub.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FullStubDto>> Add([FromBody] StubDto stub, CancellationToken cancellationToken) =>
        Ok(Mapper.Map<FullStubDto>(await Mediator.Send(new AddStubCommand(Mapper.Map<StubModel>(stub)), cancellationToken)));

    /// <summary>
    /// Adds multiple new stubs.
    /// </summary>
    /// <param name="stubs">The posted stubs.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, with the created stubs</returns>
    [HttpPost("multiple")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> AddMultiple([FromBody] IEnumerable<StubDto> stubs, CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<FullStubDto>>(
            await Mediator.Send(new AddStubsCommand(Mapper.Map<IEnumerable<StubModel>>(stubs)), cancellationToken)));

    /// <summary>
    /// Updates a given stub.
    /// </summary>
    /// <param name="stub">The posted stub.</param>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, but no content returned.</returns>
    [HttpPut("{stubId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Update([FromBody] StubDto stub, string stubId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new UpdateStubCommand(stubId, Mapper.Map<StubModel>(stub)), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get all stubs.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All stubs.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<FullStubDto>>(await Mediator.Send(new GetAllStubsQuery(), cancellationToken)));

    /// <summary>
    /// Get stub overview.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All stubs.</returns>
    [HttpGet("overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubOverviewDto>>> GetOverview(CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<FullStubOverviewDto>>(await Mediator.Send(new GetStubsOverviewQuery(), cancellationToken)));

    /// <summary>
    /// Get requests for the given stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>request results for the given stubId</returns>
    [HttpGet]
    [Route("{stubId}/requests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetRequestsByStubId([FromRoute] string stubId, CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<RequestResultDto>>(await Mediator.Send(new GetByStubIdQuery(stubId), cancellationToken)));

    /// <summary>
    /// Get a specific stub by stub identifier.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stub.</returns>
    [HttpGet]
    [Route("{stubId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<FullStubDto>> Get([FromRoute] string stubId, CancellationToken cancellationToken) =>
        Ok(Mapper.Map<FullStubDto>(await Mediator.Send(new GetStubQuery(stubId), cancellationToken)));

    /// <summary>
    /// Delete a specific stub by stub identifier.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, but not content</returns>
    [HttpDelete]
    [Route("{stubId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] string stubId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteStubCommand(stubId), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete ALL stubs. Be careful.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, but not content</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAll(CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteAllStubsCommand(), cancellationToken);
        return NoContent();
    }
}
