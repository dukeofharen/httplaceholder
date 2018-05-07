using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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
         // Enable rewind here to be able to read the posted body multiple times.
         context.Request.EnableRewind();

         context.Response.Clear();
         var response = _stubRequestExecutor.ExecuteRequest();
         context.Response.StatusCode = response.StatusCode;
         foreach (var header in response.Headers)
         {
            context.Response.Headers.Add(header.Key, header.Value);
         }

         if (response.Body != null)
         {
            await context.Response.WriteAsync(response.Body);
         }
      }
   }
}
