using System.Net;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HttPlaceholder.Web.Shared.Middleware;

/// <summary>
///     A piece of middleware for handling exceptions.
/// </summary>
public class ApiExceptionHandlingMiddleware(RequestDelegate next, IHttpContextService httpContextService)
{
    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        if (httpContextService.Path?.Contains("ph-api/") == true)
        {
            var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
            try
            {
                await next(context);
            }
            catch (ConflictException)
            {
                httpContextService.SetStatusCode(HttpStatusCode.Conflict);
            }
            catch (NotFoundException)
            {
                httpContextService.SetStatusCode(HttpStatusCode.NotFound);
            }
            catch (ForbiddenException)
            {
                httpContextService.SetStatusCode(HttpStatusCode.Forbidden);
            }
            catch (ArgumentException ex)
            {
                await WriteResponseBody(new[] { ex.Message }, HttpStatusCode.BadRequest, cancellationToken);
            }
            catch (ValidationException ex)
            {
                await WriteResponseBody(ex.ValidationErrors, HttpStatusCode.BadRequest, cancellationToken);
            }
        }
        else
        {
            await next(context);
        }
    }

    private async Task WriteResponseBody(object body, HttpStatusCode httpStatusCode,
        CancellationToken cancellationToken)
    {
        httpContextService.SetStatusCode(httpStatusCode);
        httpContextService.AddHeader(HeaderKeys.ContentType, MimeTypes.JsonMime);
        await httpContextService.WriteAsync(JsonConvert.SerializeObject(body), cancellationToken);
    }
}
