using Microsoft.AspNetCore.Mvc;

namespace Placeholder.Controllers
{
   [Route("ph-api/stubs")]
   public class StubController : Controller
   {
      [HttpPost]
      public IActionResult Add()
      {
         return Ok();
      }
   }
}
