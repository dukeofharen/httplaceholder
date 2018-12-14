using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Filters;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace HttPlaceholder.Controllers
{
    [Route("ph-api/tenants")]
    [ApiAuthorization]
    public class TenantController : Controller
    {
        private readonly ILogger<TenantController> _logger;
        private readonly IStubContainer _stubContainer;

        public TenantController(
           ILogger<TenantController> logger,
           IStubContainer stubContaner)
        {
            _logger = logger;
            _stubContainer = stubContaner;
        }

        [HttpGet]
        [Route("{tenant}/stubs")]
        public async Task<IEnumerable<FullStubModel>> GetAll(string tenant)
        {
            _logger.LogInformation($"Retrieving all stubs for tenant '{tenant}'.");
            var stubs = await _stubContainer.GetStubsAsync(tenant);
            return stubs;
        }

        [HttpDelete]
        [Route("{tenant}/stubs")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "No content")]
        public async Task<IActionResult> DeleteAll(string tenant)
        {
            _logger.LogInformation($"Deleting all stubs for tenant '{tenant}'.");
            await _stubContainer.DeleteAllStubsAsync(tenant);
            return NoContent();
        }

        [HttpPut]
        [Route("{tenant}/stubs")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "No content")]
        public async Task<IActionResult> UpdateAll(string tenant, [FromBody]IEnumerable<StubModel> stubs)
        {
            _logger.LogInformation($"Updating all stubs for tenant '{tenant}'.");
            await _stubContainer.UpdateAllStubs(tenant, stubs);
            return NoContent();
        }
    }
}
