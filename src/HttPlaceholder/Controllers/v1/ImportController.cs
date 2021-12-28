﻿using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Import.Commands;
using HttPlaceholder.Application.Import.Commands.CreateCurlStub;
using HttPlaceholder.Application.Import.Commands.CreateHarStub;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// Controller for importing data in HttPlaceholder.
/// </summary>
[Route("ph-api/import")]
[ApiAuthorization]
public class ImportController : BaseApiController
{
    /// <summary>
    /// An endpoint that is used for creating a stub (or multiple stubs) based on cURL command(s).
    /// </summary>
    /// <param name="input">The data which should be added.</param>
    /// <param name="doNotCreateStub">Whether to add the stub to the data source. If set to false, the stub is only returned but not added.</param>
    /// <returns>OK, with the generated stubs.</returns>
    [HttpPost("curl")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> CreateCurlStubs(
        [FromBody] string input,
        [FromQuery] bool doNotCreateStub) =>
        Ok(Mapper.Map<IEnumerable<FullStubDto>>(
            await Mediator.Send(new CreateCurlStubCommand(input, doNotCreateStub))));

    /// <summary>
    /// An endpoint that is used for creating stubs based on a HAR file (HTTP Archive)
    /// </summary>
    /// <param name="input">The raw HAR JSON input.</param>
    /// <param name="doNotCreateStub">Whether to add the stub to the data source. If set to false, the stub is only returned but not added.</param>
    /// <returns>OK, with the generated stubs.</returns>
    [HttpPost("har")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> CreateHarStubs(
        [FromBody] string input,
        [FromQuery] bool doNotCreateStub) =>
        Ok(Mapper.Map<IEnumerable<FullStubDto>>(
            await Mediator.Send(new CreateHarStubCommand(input, doNotCreateStub))));
}
