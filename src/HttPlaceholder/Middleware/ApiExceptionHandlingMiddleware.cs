using System;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Exceptions;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Middleware
{
   public class ApiExceptionHandlingMiddleware
   {
      private readonly RequestDelegate _next;

      public ApiExceptionHandlingMiddleware(RequestDelegate next)
      {
         _next = next;
      }

      public async Task Invoke(HttpContext context)
      {
         if (context.Request.Path.Value.Contains("ph-api/"))
         {
            try
            {
               await _next(context);
            }
            catch (ConflictException)
            {
               context.Response.StatusCode = (int)HttpStatusCode.Conflict;
            }
            catch (Exception)
            {
               throw;
            }
         }
         else
         {
            await _next(context);
         }
      }
   }
}
