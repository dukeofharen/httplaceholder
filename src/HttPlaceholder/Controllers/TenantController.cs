using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("ph-api/tenants")]
    public class TenantController : BaseApiController
    {
        private readonly ILogger<TenantController> _logger;
        private readonly IStubContainer _stubContainer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="stubContaner"></param>
        public TenantController(
           ILogger<TenantController> logger,
           IStubContainer stubContaner)
        {
            _logger = logger;
            _stubContainer = stubContaner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{tenant}/stubs")]
        public async Task<IEnumerable<FullStubModel>> GetAll(string tenant)
        {
            _logger.LogInformation($"Retrieving all stubs for tenant '{tenant}'.");
            var stubs = await _stubContainer.GetStubsAsync(tenant);
            return stubs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenant"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{tenant}/stubs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> DeleteAll(string tenant)
        {
            _logger.LogInformation($"Deleting all stubs for tenant '{tenant}'.");
            await _stubContainer.DeleteAllStubsAsync(tenant);
            return NoContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenant"></param>
        /// <param name="stubs"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{tenant}/stubs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpdateAll(string tenant, [FromBody]IEnumerable<StubModel> stubs)
        {
            _logger.LogInformation($"Updating all stubs for tenant '{tenant}'.");
            await _stubContainer.UpdateAllStubs(tenant, stubs);
            return NoContent();
        }
    }
}
