﻿using System;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace HttPlaceholder.Middleware;

/// <summary>
/// A piece of middleware for handling exceptions.
/// </summary>
public class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextService _httpContextService;

    /// <summary>
    /// Constructs an <see cref="ApiExceptionHandlingMiddleware"/> instance.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="httpContextService"></param>
    public ApiExceptionHandlingMiddleware(RequestDelegate next, IHttpContextService httpContextService)
    {
        _next = next;
        _httpContextService = httpContextService;
    }

    /// <summary>
    /// Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        if (_httpContextService.Path?.Contains("ph-api/") == true)
        {
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
                _httpContextService.SetStatusCode(HttpStatusCode.BadRequest);
                _httpContextService.AddHeader("Content-Type", Constants.JsonMime);
                await _httpContextService.WriteAsync(JsonConvert.SerializeObject(new[] {ex.Message}));
            }
            catch (ValidationException ex)
            {
                _httpContextService.SetStatusCode(HttpStatusCode.BadRequest);
                _httpContextService.AddHeader("Content-Type", Constants.JsonMime);
                await _httpContextService.WriteAsync(JsonConvert.SerializeObject(ex.ValidationErrors));
            }
        }
        else
        {
            await _next(context);
        }
    }
}
