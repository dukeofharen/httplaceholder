using System;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
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
                catch (NotFoundException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                catch (ForbiddenException)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                }
                catch (ArgumentException ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsJsonAsync(new string[] {ex.Message});
                }
                catch (ValidationException ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsJsonAsync(ex.ValidationErrors);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
