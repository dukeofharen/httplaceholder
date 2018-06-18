using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Placeholder.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Placeholder.Exceptions;
using Placeholder.Implementation;

namespace Placeholder.Middleware
{
   public class StubHandlingMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly IHttpContextService _httpContextService;
      private readonly ILogger<StubHandlingMiddleware> _logger;
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IStubRequestExecutor _stubRequestExecutor;

      public StubHandlingMiddleware(
         RequestDelegate next,
         IHttpContextService httpContextService,
         ILogger<StubHandlingMiddleware> logger,
         IRequestLoggerFactory requestLoggerFactory,
         IStubRequestExecutor stubRequestExecutor)
      {
         _next = next;
         _httpContextService = httpContextService;
         _logger = logger;
         _requestLoggerFactory = requestLoggerFactory;
         _stubRequestExecutor = stubRequestExecutor;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context.Request.Path.StartsWithSegments("/ph-api"))
         {
            await _next(context);
            return;
         }

         const string correlationHeaderKey = "X-Placeholder-Correlation";
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         string correlation = Guid.NewGuid().ToString();
         requestLogger.Log($"========== BEGINNING REQUEST {correlation} ==========");
         try
         {
            // Enable rewind here to be able to read the posted body multiple times.
            context.Request.EnableRewind();

            // Log the request here
            requestLogger.Log($"Request URL ({_httpContextService.Method}): {_httpContextService.DisplayUrl}");
            requestLogger.Log($"Request body: {_httpContextService.GetBody()}");
            string headerString = string.Join(", ", _httpContextService
               .GetHeaders()
               .Select(h => $"{h.Key} = {h.Value}"));
            requestLogger.Log($"Request headers: {headerString}");

            context.Response.Clear();
            context.Response.Headers.TryAdd(correlationHeaderKey, correlation);
            var response = await _stubRequestExecutor.ExecuteRequestAsync();
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
         catch (RequestValidationException e)
         {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers.TryAdd(correlationHeaderKey, correlation);
            requestLogger.Log($"Request validation exception thrown: {e.Message}");
         }
         catch (Exception e)
         {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Headers.TryAdd(correlationHeaderKey, correlation);
            requestLogger.Log($"Unexpected exception thrown: {e}");
         }

         requestLogger.Log($"========== FINISHING REQUEST {correlation} ==========");
         _logger.LogInformation(requestLogger.FullMessage);
      }
   }
}
