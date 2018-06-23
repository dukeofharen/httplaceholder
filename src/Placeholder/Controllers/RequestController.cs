using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Placeholder.Filters;
using Placeholder.Implementation;
using Placeholder.Models;

namespace Placeholder.Controllers
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
      public async Task<object> GetByStubId(string stubId)
      {
         var requests = await _stubContainer.GetRequestResultsByStubIdAsync(stubId);
         return requests;
      }
   }
}
