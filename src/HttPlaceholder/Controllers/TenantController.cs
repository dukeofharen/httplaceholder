using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// Tenant Controller
    /// </summary>
    [Route("ph-api/tenants")]
    public class TenantController : BaseApiController
    {
        private readonly ILogger<TenantController> _logger;
        private readonly IStubContext _stubContext;

        /// <summary>
        /// The tenant controller.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="stubContext"></param>
        public TenantController(
           ILogger<TenantController> logger,
           IStubContext stubContext)
        {
            _logger = logger;
            _stubContext = stubContext;
        }

        /// <summary>
        /// Gets all stubs in a specific tenant.
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns>All stubs in the tenant.</returns>
        [HttpGet]
        [Route("{tenant}/stubs")]
        public async Task<IEnumerable<FullStubModel>> GetAll(string tenant)
        {
            _logger.LogInformation($"Retrieving all stubs for tenant '{tenant}'.");
            var stubs = await _stubContext.GetStubsAsync(tenant);
            return stubs;
        }

        /// <summary>
        /// Deletes all stubs in a specific tenant.
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns>OK, but no content</returns>
        [HttpDelete]
        [Route("{tenant}/stubs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteAll(string tenant)
        {
            _logger.LogInformation($"Deleting all stubs for tenant '{tenant}'.");
            await _stubContext.DeleteAllStubsAsync(tenant);
            return NoContent();
        }

        /// <summary>
        /// Updates the stubs in a specific tenant with the posted values.
        /// If a stub that is currently available in a tenant isn't sent in the request,
        /// it will be deleted.
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="stubs"></param>
        /// <returns>OK, but no content</returns>
        [HttpPut]
        [Route("{tenant}/stubs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateAll(string tenant, [FromBody]IEnumerable<StubModel> stubs)
        {
            _logger.LogInformation($"Updating all stubs for tenant '{tenant}'.");
            await _stubContext.UpdateAllStubs(tenant, stubs);
            return NoContent();
        }
    }
}
