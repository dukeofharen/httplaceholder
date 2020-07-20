using System.Collections.Generic;
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

// ReSharper disable InconsistentNaming

namespace HttPlaceholder.Controllers.v1
{
    /// <summary>
    /// Tenant Controller
    /// </summary>
    [Route("ph-api/tenants")]
    [ApiAuthorization]
    public class TenantController : BaseApiController
    {
        /// <summary>
        /// Gets all available tenant names.
        /// </summary>
        /// <returns>All available tenant names.</returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetTenantNames() =>
            Ok(await Mediator.Send(new GetTenantNamesQuery()));

        /// <summary>
        /// Gets all stubs in a specific tenant.
        /// </summary>
        /// <returns>All stubs in the tenant.</returns>
        [HttpGet]
        [Route("{Tenant}/stubs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FullStubDto>>> GetAll([FromRoute]GetStubsInTenantQuery query) =>
            Ok(Mapper.Map<IEnumerable<FullStubDto>>(await Mediator.Send(query)));

        /// <summary>
        /// Deletes all stubs in a specific tenant.
        /// </summary>
        /// <returns>OK, but no content</returns>
        [HttpDelete]
        [Route("{Tenant}/stubs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteAll([FromRoute]DeleteStubsInTenantCommand command)
        {
            await Mediator.Send(command);
            return NoContent();
        }

        /// <summary>
        /// Updates the stubs in a specific tenant with the posted values.
        /// If a stub that is currently available in a tenant isn't sent in the request,
        /// it will be deleted.
        /// </summary>
        /// <param name="Tenant"></param>
        /// <param name="stubs"></param>
        /// <returns>OK, but no content</returns>
        [HttpPut]
        [Route("{Tenant}/stubs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateAll(string Tenant, [FromBody]IEnumerable<StubDto> stubs)
        {
            await Mediator.Send(new UpdateStubsInTenantCommand { Tenant = Tenant, Stubs = Mapper.Map<IEnumerable<StubModel>>(stubs) });
            return NoContent();
        }
    }
}
