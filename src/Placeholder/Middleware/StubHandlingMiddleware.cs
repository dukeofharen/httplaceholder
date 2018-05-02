using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Placeholder.Middleware
{
   class StubHandlingMiddleware
   {
      public StubHandlingMiddleware(RequestDelegate next)
      {
      }

      public async Task Invoke(HttpContext context)
      {
         context.Response.Clear();
         context.Response.StatusCode = (int)HttpStatusCode.OK;
         await context.Response.WriteAsync("HELP!");
      }
   }
}
