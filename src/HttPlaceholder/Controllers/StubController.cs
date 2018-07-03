using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HttPlaceholder.Filters;
using HttPlaceholder.Implementation;
using HttPlaceholder.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HttPlaceholder.Controllers
{
   [Route("ph-api/stubs")]
   [ApiAuthorization]
   public class StubController : Controller
   {
      private readonly IStubContainer _stubContainer;

      public StubController(IStubContainer stubContaner)
      {
         _stubContainer = stubContaner;
      }

      [HttpPost]
      [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "OK, but no content returned")]
      public async Task<IActionResult> Add([FromBody]StubModel stubModel)
      {
         // Delete stub with same ID.
         await _stubContainer.DeleteStubAsync(stubModel.Id);

         await _stubContainer.AddStubAsync(stubModel);
         return NoContent();
      }

      [HttpGet]
      public async Task<IEnumerable<StubModel>> GetAll()
      {
         var stubs = await _stubContainer.GetStubsAsync();
         return stubs;
      }

      [HttpGet]
      [Route("{stubId}")]
      [SwaggerResponse((int)HttpStatusCode.OK, Description = "OK")]
      [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Stub not found")]
      public async Task<object> Get([FromRoute]string stubId)
      {
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
         bool result = await _stubContainer.DeleteStubAsync(stubId);
         return result ? (IActionResult)NoContent() : NotFound();
      }
   }
}
