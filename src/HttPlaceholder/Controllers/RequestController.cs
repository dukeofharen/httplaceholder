using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HttPlaceholder.Filters;
using HttPlaceholder.BusinessLogic;
using HttPlaceholder.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace HttPlaceholder.Controllers
{
   [Route("ph-api/requests")]
   [ApiAuthorization]
   public class RequestController : Controller
   {
      private readonly IStubContainer _stubContainer;

      public RequestController(IStubContainer stubContaner)
      {
         _stubContainer = stubContaner;
      }

      [HttpGet]
      public async Task<IEnumerable<RequestResultModel>> GetAll()
      {
         var requests = await _stubContainer.GetRequestResultsAsync();
         return requests;
      }

      [HttpGet]
      [Route("{stubId}")]
      public async Task<IEnumerable<RequestResultModel>> GetByStubId([FromRoute]string stubId)
      {
         var requests = await _stubContainer.GetRequestResultsByStubIdAsync(stubId);
         return requests;
      }

      [HttpDelete]
      [SwaggerResponse((int)HttpStatusCode.NoContent, Description = "OK, but no content returned")]
      public async Task<IActionResult> DeleteAll()
      {
         await _stubContainer.DeleteAllRequestResultsAsync();
         return NoContent();
      }
   }
}
