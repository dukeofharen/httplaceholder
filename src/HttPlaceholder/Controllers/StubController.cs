using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Filters;
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
        /// 
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
        /// 
        /// </summary>
        /// <param name="stubModel"></param>
        /// <returns></returns>
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FullStubModel>>> GetAll()
        {
            _logger.LogInformation("Retrieving all stubs.");
            var stubs = await _stubContainer.GetStubsAsync();
            return Ok(stubs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stubId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{stubId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        //  [SwaggerResponse((int)HttpStatusCode.OK, Description = "OK", Type = typeof(StubModel))]
        //    [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Stub not found")]
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
        /// 
        /// </summary>
        /// <param name="stubId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{stubId}")]
        // [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Stub not found")]
        // [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "OK, but no content returned")]
        public async Task<ActionResult> Delete([FromRoute]string stubId)
        {
            _logger.LogInformation($"Deleting stub with ID '{stubId}'");
            bool result = await _stubContainer.DeleteStubAsync(stubId);
            return result ? NoContent() : (ActionResult)NotFound();
        }
    }
}