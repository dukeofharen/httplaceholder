using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Application.Tenants.Commands;
using HttPlaceholder.Application.Tenants.Queries;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Authorization;
using HttPlaceholder.Web.Shared.Dto.v1.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
///     The tenant controller.
/// </summary>
[Route("ph-api/tenants")]
[ApiAuthorization]
public class TenantController : BaseApiController
{
    /// <summary>
    ///     Gets all available tenant names.
    /// </summary>
    /// <returns>All available tenant names.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetTenantNames() =>
        Ok(await Send(new GetTenantNamesQuery()));

    /// <summary>
    ///     Gets all stubs in a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <returns>All stubs in the tenant.</returns>
    [HttpGet]
    [Route("{tenant}/stubs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> GetAll([FromRoute] string tenant) =>
        Ok(Map<IEnumerable<FullStubDto>>(await Send(new GetStubsInTenantQuery(tenant))));

    /// <summary>
    ///     Deletes all stubs in a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <returns>OK, but no content</returns>
    [HttpDelete]
    [Route("{tenant}/stubs")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteAll([FromRoute] string tenant)
    {
        await Send(new DeleteStubsInTenantCommand(tenant));
        return NoContent();
    }

    /// <summary>
    ///     Updates the stubs in a specific tenant with the posted values.
    ///     If a stub that is currently available in a tenant isn't sent in the request, it will be deleted.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="stubs">The stubs to update.</param>
    /// <returns>OK, but no content</returns>
    [HttpPut]
    [Route("{tenant}/stubs")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> UpdateAll([FromRoute] string tenant, [FromBody] IEnumerable<StubDto> stubs)
    {
        await Send(new UpdateStubsInTenantCommand(Map<IEnumerable<StubModel>>(stubs), tenant));
        return NoContent();
    }
}
