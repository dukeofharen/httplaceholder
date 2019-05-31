using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Controllers
{
    /// <summary>
    /// Controller for request 
    /// </summary>
    [Route("ph-api/requests")]
    public class RequestController : BaseApiController
    {
        private readonly ILogger<RequestController> _logger;
        private readonly IStubContainer _stubContainer;

        /// <summary>
        /// Constructor for RequestController
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="stubContaner"></param>
        public RequestController(
           ILogger<RequestController> logger,
           IStubContainer stubContaner)
        {
            _logger = logger;
            _stubContainer = stubContaner;
        }

        /// <summary>
        /// Get all Requests.
        /// </summary>
        /// <returns>All request results</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestResultModel>>> GetAll()
        {
            _logger.LogInformation($"Retrieving all requests.");
            var requests = await _stubContainer.GetRequestResultsAsync();
            return Ok(requests);
        }

        /// <summary>
        /// Get requests for the given stub ID.
        /// </summary>
        /// <param name="stubId">stub identifier</param>
        /// <returns>request results for the given stubId</returns>
        [HttpGet]
        [Route("{stubId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestResultModel>>> GetByStubId([FromRoute]string stubId)
        {
            _logger.LogInformation($"Retrieving requests for stub ID '{stubId}'");
            var requests = await _stubContainer.GetRequestResultsByStubIdAsync(stubId);
            return Ok(requests);
        }

        /// <summary>
        /// Delete all requests. This call flushes all the requests.
        /// </summary>
        /// <returns>OK, but no content returned</returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAll()
        {
            _logger.LogInformation("Deleting all requests.");
            await _stubContainer.DeleteAllRequestResultsAsync();
            return NoContent();
        }
    }
}