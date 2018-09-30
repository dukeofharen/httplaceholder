using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HttPlaceholder.Filters;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Models;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Controllers
{
   [Route("ph-api/stubs")]
   [ApiAuthorization]
   public class StubController : Controller
   {
      private readonly ILogger<StubController> _logger;
      private readonly IStubContainer _stubContainer;

      public StubController(
         ILogger<StubController> logger,
         IStubContainer stubContaner)
      {
         _logger = logger;
         _stubContainer = stubContaner;
      }

      [HttpPost]
      [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "OK, but no content returned")]
      [SwaggerResponse((int)HttpStatusCode.Conflict, Description = "The stub is already available in another stub source.")]
      public async Task<IActionResult> Add([FromBody]StubModel stubModel)
      {
         _logger.LogInformation($"Adding new stub '{stubModel}'");

         // Delete stub with same ID.
         await _stubContainer.DeleteStubAsync(stubModel.Id);

         await _stubContainer.AddStubAsync(stubModel);
         return NoContent();
      }

      [HttpGet]
      public async Task<IEnumerable<StubModel>> GetAll()
      {
         _logger.LogInformation("Retrieving all stubs.");
         var stubs = await _stubContainer.GetStubsAsync();
         return stubs;
      }

      [HttpGet]
      [Route("{stubId}")]
      [SwaggerResponse((int)HttpStatusCode.OK, Description = "OK", Type = typeof(StubModel))]
      [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Stub not found")]
      public async Task<object> Get([FromRoute]string stubId)
      {
         _logger.LogInformation($"Retrieving stub with ID '{stubId}'.");
         var result = await _stubContainer.GetStubAsync(stubId);
         if (result == null)
         {
            return NotFound();
         }

         return result;
      }

      [HttpDelete]
      [Route("{stubId}")]
      [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Stub not found")]
      [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "OK, but no content returned")]
      public async Task<IActionResult> Delete([FromRoute]string stubId)
      {
         _logger.LogInformation($"Deleting stub with ID '{stubId}'");
         bool result = await _stubContainer.DeleteStubAsync(stubId);
         return result ? (IActionResult)NoContent() : NotFound();
      }
   }
}
