﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Requests.Queries;
using HttPlaceholder.Application.Stubs.Commands;
using HttPlaceholder.Application.Stubs.Queries;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Authorization;
using HttPlaceholder.Web.Shared.Dto.v1.Requests;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     The stub controller.
/// </summary>
[Route("ph-api/stubs")]
[ApiAuthorization]
public class StubController : BaseApiController
{
    /// <summary>
    ///     Adds a new stub.
    /// </summary>
    /// <param name="stub">The posted stub.</param>
    /// <returns>OK, with the created stub.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FullStubDto>> Add([FromBody] StubDto stub) =>
        Ok(Map<FullStubDto>(await Send(new AddStubCommand(Map<StubModel>(stub)))));

    /// <summary>
    ///     Adds multiple new stubs.
    /// </summary>
    /// <param name="stubs">The posted stubs.</param>
    /// <returns>OK, with the created stubs</returns>
    [HttpPost("multiple")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> AddMultiple([FromBody] IEnumerable<StubDto> stubs) =>
        Ok(Map<IEnumerable<FullStubDto>>(
            await Send(new AddStubsCommand(Map<IEnumerable<StubModel>>(stubs)))));

    /// <summary>
    ///     Updates a given stub.
    /// </summary>
    /// <param name="stub">The posted stub.</param>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>OK, but no content returned.</returns>
    [HttpPut("{stubId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Update([FromBody] StubDto stub, string stubId)
    {
        await Send(new UpdateStubCommand(stubId, Map<StubModel>(stub)));
        return NoContent();
    }

    /// <summary>
    ///     Get all stubs.
    /// </summary>
    /// <returns>All stubs.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> GetAll() =>
        Ok(Map<IEnumerable<FullStubDto>>(await Send(new GetAllStubsQuery())));

    /// <summary>
    ///     Get stub overview.
    /// </summary>
    /// <returns>All stubs.</returns>
    [HttpGet("overview")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubOverviewDto>>> GetOverview() =>
        Ok(Map<IEnumerable<FullStubOverviewDto>>(await Send(new GetStubsOverviewQuery())));

    /// <summary>
    ///     Get requests for the given stub ID.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>request results for the given stubId</returns>
    [HttpGet]
    [Route("{stubId}/requests")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RequestResultDto>>> GetRequestsByStubId([FromRoute] string stubId) =>
        Ok(Map<IEnumerable<RequestResultDto>>(await Send(new GetByStubIdQuery(stubId))));

    /// <summary>
    ///     Get a specific stub by stub identifier.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>The stub.</returns>
    [HttpGet]
    [Route("{stubId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<FullStubDto>> Get([FromRoute] string stubId) =>
        Ok(Map<FullStubDto>(await Send(new GetStubQuery(stubId))));

    /// <summary>
    ///     Delete a specific stub by stub identifier.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>OK, but not content</returns>
    [HttpDelete]
    [Route("{stubId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] string stubId)
    {
        await Send(new DeleteStubCommand(stubId));
        return NoContent();
    }

    /// <summary>
    ///     Delete ALL stubs. Be careful.
    /// </summary>
    /// <returns>OK, but not content</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> DeleteAll()
    {
        await Send(new DeleteAllStubsCommand());
        return NoContent();
    }
}
