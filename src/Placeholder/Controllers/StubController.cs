using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Placeholder.Filters;
using Placeholder.Implementation;
using Placeholder.Models;

namespace Placeholder.Controllers
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
      public async Task<object> Get(string stubId)
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
      public async Task<IActionResult> Delete(string stubId)
      {
         bool result = await _stubContainer.DeleteStubAsync(stubId);
         return result ? (IActionResult)NoContent() : NotFound();
      }
   }
}
