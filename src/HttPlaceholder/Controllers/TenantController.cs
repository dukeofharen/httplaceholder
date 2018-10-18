using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Filters;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        public async Task<IEnumerable<StubModel>> GetAll(string tenant)
        {
            _logger.LogInformation("Retrieving all stubs.");
            var stubs = await _stubContainer.GetStubsAsync(tenant);
            return stubs;
        }
    }
}
