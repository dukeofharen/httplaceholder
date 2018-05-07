using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation;
using Placeholder.Implementation.Services;

namespace Placeholder.Middleware
{
   public class StubHandlingMiddleware
   {
      private readonly IHttpContextService _httpContextService;
      private readonly ILogger<StubHandlingMiddleware> _logger;
      private readonly IStubRequestExecutor _stubRequestExecutor;

      public StubHandlingMiddleware(
         RequestDelegate next,
         IHttpContextService httpContextService,
         ILogger<StubHandlingMiddleware> logger,
         IStubRequestExecutor stubRequestExecutor)
      {
         _httpContextService = httpContextService;
         _logger = logger;
         _stubRequestExecutor = stubRequestExecutor;
      }

      public async Task Invoke(HttpContext context)
      {
         // Enable rewind here to be able to read the posted body multiple times.
         context.Request.EnableRewind();

         // Log the request here
         _logger.LogInformation($"Request URL ({_httpContextService.Method}): {_httpContextService.DisplayUrl}");
         _logger.LogInformation($"Request body: {_httpContextService.GetBody()}");
         string headerString = string.Join(", ", _httpContextService
            .GetHeaders()
            .Select(h => $"{h.Key} = {h.Value}"));
         _logger.LogInformation($"Request headers: {headerString}");

         context.Response.Clear();
         var response = _stubRequestExecutor.ExecuteRequest();
         context.Response.StatusCode = response.StatusCode;
         foreach (var header in response.Headers)
         {
            context.Response.Headers.Add(header.Key, header.Value);
         }

         if (response.Body != null)
         {
            await context.Response.Body.WriteAsync(response.Body, 0, response.Body.Length);
         }
      }
   }
}
