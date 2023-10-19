using System.Threading.Tasks;
using HttPlaceholder.Application.Export.Queries.ExportRequest;
using HttPlaceholder.Domain.Enums;
using HttPlaceholder.Web.Shared.Dto.v1.Export;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     Controller for exporting data in HttPlaceholder.
/// </summary>
[Route("ph-api/export")]
public class ExportController : BaseApiController
{
    /// <summary>
    ///     An endpoint for exporting a request into a specific format.
    /// </summary>
    /// <param name="requestId">The request ID.</param>
    /// <param name="type">The request export type.</param>
    /// <returns>The exported result.</returns>
    [HttpGet("requests/{requestId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RequestExportResultDto>> ExportRequest(
        [FromRoute] string requestId,
        [FromQuery] RequestExportType type) =>
        Ok(new RequestExportResultDto
        {
            RequestExportType = type, Result = await Mediator.Send(new ExportRequestQuery(requestId, type))
        });
}
