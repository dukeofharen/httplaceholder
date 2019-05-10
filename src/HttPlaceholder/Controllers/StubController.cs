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
    /// Stub Controller
    /// </summary>
    [Route("ph-api/stubs")]
    public class StubController : BaseApiController
    {
        private readonly ILogger<StubController> _logger;
        private readonly IStubContainer _stubContainer;

        /// <summary>
        /// The stub controller.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="stubContaner"></param>
        public StubController(
           ILogger<StubController> logger,
           IStubContainer stubContaner)
        {
            _logger = logger;
            _stubContainer = stubContaner;
        }

        /// <summary>
        /// Adds a new stub.
        /// </summary>
        /// <param name="stubModel"></param>
        /// <returns>OK, but no content returned</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Add([FromBody]StubModel stubModel)
        {
            _logger.LogInformation($"Adding new stub '{stubModel}'");

            // Delete stub with same ID.
            await _stubContainer.DeleteStubAsync(stubModel.Id);

            await _stubContainer.AddStubAsync(stubModel);
            return NoContent();
        }

        /// <summary>
        /// Get all stubs.
        /// </summary>
        /// <returns>All stubs.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FullStubModel>>> GetAll()
        {
            _logger.LogInformation("Retrieving all stubs.");
            var stubs = await _stubContainer.GetStubsAsync();
            return Ok(stubs);
        }

        /// <summary>
        /// Get a specific stub by stub identifier.
        /// </summary>
        /// <param name="stubId"></param>
        /// <returns>The stub.</returns>
        [HttpGet]
        [Route("{stubId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<FullStubModel>> Get([FromRoute]string stubId)
        {
            _logger.LogInformation($"Retrieving stub with ID '{stubId}'.");
            var result = await _stubContainer.GetStubAsync(stubId);
            if (result == null)
            {
                return NotFound();
            }

            return result;
        }

        /// <summary>
        /// Delete a specific stub by stub identifier.
        /// </summary>
        /// <param name="stubId"></param>
        /// <returns>OK, but not content</returns>
        [HttpDelete]
        [Route("{stubId}")]
        public async Task<ActionResult> Delete([FromRoute]string stubId)
        {
            _logger.LogInformation($"Deleting stub with ID '{stubId}'");
            bool result = await _stubContainer.DeleteStubAsync(stubId);
            return result ? NoContent() : (ActionResult)NotFound();
        }
    }
}