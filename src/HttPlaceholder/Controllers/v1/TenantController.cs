using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant;
using HttPlaceholder.Application.Tenants.Commands.UpdateStubsInTenant;
using HttPlaceholder.Application.Tenants.Queries.GetStubsInTenant;
using HttPlaceholder.Application.Tenants.Queries.GetTenantNames;
using HttPlaceholder.Authorization;
using HttPlaceholder.Domain;
using HttPlaceholder.Dto.v1.Stubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// The tenant controller.
/// </summary>
[Route("ph-api/tenants")]
[ApiAuthorization]
public class TenantController : BaseApiController
{
    /// <summary>
    /// Gets all available tenant names.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All available tenant names.</returns>
    [HttpGet]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<string>>> GetTenantNames(CancellationToken cancellationToken) =>
        Ok(await Mediator.Send(new GetTenantNamesQuery(), cancellationToken));

    /// <summary>
    /// Gets all stubs in a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>All stubs in the tenant.</returns>
    [HttpGet]
    [Route("{tenant}/stubs")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FullStubDto>>> GetAll([FromRoute] string tenant, CancellationToken cancellationToken) =>
        Ok(Mapper.Map<IEnumerable<FullStubDto>>(await Mediator.Send(new GetStubsInTenantQuery(tenant), cancellationToken)));

    /// <summary>
    /// Deletes all stubs in a specific tenant.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, but no content</returns>
    [HttpDelete]
    [Route("{tenant}/stubs")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> DeleteAll([FromRoute] string tenant, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteStubsInTenantCommand(tenant), cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Updates the stubs in a specific tenant with the posted values.
    /// If a stub that is currently available in a tenant isn't sent in the request, it will be deleted.
    /// </summary>
    /// <param name="tenant">The tenant.</param>
    /// <param name="stubs">The stubs to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>OK, but no content</returns>
    [HttpPut]
    [Route("{tenant}/stubs")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> UpdateAll([FromRoute] string tenant, [FromBody] IEnumerable<StubDto> stubs, CancellationToken cancellationToken)
    {
        await Mediator.Send(new UpdateStubsInTenantCommand(Mapper.Map<IEnumerable<StubModel>>(stubs), tenant), cancellationToken);
        return NoContent();
    }
}
