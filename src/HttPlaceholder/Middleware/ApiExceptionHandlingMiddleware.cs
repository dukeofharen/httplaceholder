using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HttPlaceholder.Middleware;

/// <summary>
///     A piece of middleware for handling exceptions.
/// </summary>
public class ApiExceptionHandlingMiddleware
{
    private readonly IHttpContextService _httpContextService;
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Constructs an <see cref="ApiExceptionHandlingMiddleware" /> instance.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="httpContextService"></param>
    public ApiExceptionHandlingMiddleware(RequestDelegate next, IHttpContextService httpContextService)
    {
        _next = next;
        _httpContextService = httpContextService;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        if (_httpContextService.Path?.Contains("ph-api/") == true)
        {
            var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
            try
            {
                await _next(context);
            }
            catch (ConflictException)
            {
                _httpContextService.SetStatusCode(HttpStatusCode.Conflict);
            }
            catch (NotFoundException)
            {
                _httpContextService.SetStatusCode(HttpStatusCode.NotFound);
            }
            catch (ForbiddenException)
            {
                _httpContextService.SetStatusCode(HttpStatusCode.Forbidden);
            }
            catch (ArgumentException ex)
            {
                await WriteResponseBody(new[] {ex.Message}, HttpStatusCode.BadRequest, cancellationToken);
            }
            catch (ValidationException ex)
            {
                await WriteResponseBody(ex.ValidationErrors, HttpStatusCode.BadRequest, cancellationToken);
            }
        }
        else
        {
            await _next(context);
        }
    }

    private async Task WriteResponseBody(object body, HttpStatusCode httpStatusCode,
        CancellationToken cancellationToken)
    {
        _httpContextService.SetStatusCode(httpStatusCode);
        _httpContextService.AddHeader(HeaderKeys.ContentType, MimeTypes.JsonMime);
        await _httpContextService.WriteAsync(JsonConvert.SerializeObject(body), cancellationToken);
    }
}
