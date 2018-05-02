using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Placeholder.Implementation;

namespace Placeholder.Middleware
{
   public class StubHandlingMiddleware
   {
      private readonly IStubRequestExecutor _stubRequestExecutor;

      public StubHandlingMiddleware(
         RequestDelegate next,
         IStubRequestExecutor stubRequestExecutor)
      {
         _stubRequestExecutor = stubRequestExecutor;
      }

      public async Task Invoke(HttpContext context)
      {
         context.Response.Clear();
         var response = await _stubRequestExecutor.ExecuteRequestAsync();
         context.Response.StatusCode = (int)response.StatusCode;
         foreach (var header in response.Headers)
         {
            context.Response.Headers.Add(header.Key, header.Value);
         }

         await context.Response.WriteAsync(response.Body);
      }
   }
}
