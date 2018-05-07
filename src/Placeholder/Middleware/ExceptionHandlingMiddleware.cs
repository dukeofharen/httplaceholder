using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Placeholder.Exceptions;

namespace Placeholder.Middleware
{
   public class ExceptionHandlingMiddleware
   {
      private readonly ILogger<ExceptionHandlingMiddleware> _logger;
      private readonly RequestDelegate _next;

      public ExceptionHandlingMiddleware(
         ILogger<ExceptionHandlingMiddleware> logger,
         RequestDelegate next)
      {
         _logger = logger;
         _next = next;
      }

      public async Task Invoke(HttpContext context)
      {
         try
         {
            await _next(context);
         }
         catch (Exception e)
         {
            if (e is RequestValidationException requestValidationException)
            {
               context.Response.Clear();
               context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
               _logger.LogInformation($"Request validation exception thrown: {requestValidationException.Message}");
            }
            else
            {
               throw;
            }
         }
      }
   }
}
