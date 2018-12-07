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
    [Route("ph-api/requests")]
    [ApiAuthorization]
    public class RequestController : Controller
    {
        private readonly ILogger<RequestController> _logger;
        private readonly IStubContainer _stubContainer;

        public RequestController(
           ILogger<RequestController> logger,
           IStubContainer stubContaner)
        {
            _logger = logger;
            _stubContainer = stubContaner;
        }

        [HttpGet]
        public async Task<IEnumerable<RequestResultModel>> GetAll()
        {
            _logger.LogInformation($"Retrieving all requests.");
            var requests = await _stubContainer.GetRequestResultsAsync();
            return requests;
        }

        [HttpGet]
        [Route("{stubId}")]
        public async Task<IEnumerable<RequestResultModel>> GetByStubId([FromRoute]string stubId)
        {
            _logger.LogInformation($"Retrieving requests for stub ID '{stubId}'");
            var requests = await _stubContainer.GetRequestResultsByStubIdAsync(stubId);
            return requests;
        }

        [HttpDelete]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "OK, but no content returned")]
        public async Task<IActionResult> DeleteAll()
        {
            _logger.LogInformation("Deleting all requests.");
            await _stubContainer.DeleteAllRequestResultsAsync();
            return NoContent();
        }
    }
}