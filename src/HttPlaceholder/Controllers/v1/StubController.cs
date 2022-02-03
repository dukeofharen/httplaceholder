using System.Collections.Generic;
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
    /// <returns>OK, with the created stub.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FullStubDto>> Add([FromBody] StubDto stub) =>
        Ok(Mapper.Map<FullStubDto>(await Mediator.Send(new AddStubCommand(Mapper.Map<StubModel>(stub)))));

    /// <summary>
    /// Adds multiple new stubs.
    /// </summary>
    /// <param name="stubs">The posted stubs.</param>
    /// <returns>OK, with the created stubs</returns>
    [HttpPost("multiple")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> AddMultiple([FromBody] IEnumerable<StubDto> stubs) =>
        Ok(Mapper.Map<IEnumerable<FullStubDto>>(
            await Mediator.Send(new AddStubsCommand(Mapper.Map<IEnumerable<StubModel>>(stubs)))));

    /// <summary>
    /// Updates a given stub.
    /// </summary>
    /// <param name="stub">The posted stub.</param>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>OK, but no content returned</returns>
    [HttpPut("{stubId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Update([FromBody] StubDto stub, string stubId)
    {
        await Mediator.Send(new UpdateStubCommand(stubId, Mapper.Map<StubModel>(stub)));
        return NoContent();
    }

    /// <summary>
    /// Get all stubs.
    /// </summary>
    /// <returns>All stubs.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> GetAll() =>
        Ok(Mapper.Map<IEnumerable<FullStubDto>>(await Mediator.Send(new GetAllStubsQuery())));

    /// <summary>
    /// Get stub overview.
    /// </summary>
    /// <returns>All stubs.</returns>
    [HttpGet("overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubOverviewDto>>> GetOverview() =>
        Ok(Mapper.Map<IEnumerable<FullStubOverviewDto>>(await Mediator.Send(new GetStubsOverviewQuery())));

    /// <summary>
    /// Get requests for the given stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>request results for the given stubId</returns>
    [HttpGet]
    [Route("{stubId}/requests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetRequestsByStubId([FromRoute] string stubId) =>
        Ok(Mapper.Map<IEnumerable<RequestResultDto>>(await Mediator.Send(new GetByStubIdQuery(stubId))));

    /// <summary>
    /// Get a specific stub by stub identifier.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>The stub.</returns>
    [HttpGet]
    [Route("{stubId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<FullStubDto>> Get([FromRoute] string stubId) =>
        Ok(Mapper.Map<FullStubDto>(await Mediator.Send(new GetStubQuery(stubId))));

    /// <summary>
    /// Delete a specific stub by stub identifier.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>OK, but not content</returns>
    [HttpDelete]
    [Route("{stubId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] string stubId)
    {
        await Mediator.Send(new DeleteStubCommand(stubId));
        return NoContent();
    }

    /// <summary>
    /// Delete ALL stubs. Be careful.
    /// </summary>
    /// <returns>OK, but not content</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAll()
    {
        await Mediator.Send(new DeleteAllStubsCommand());
        return NoContent();
    }
}
