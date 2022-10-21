using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Middleware;

/// <summary>
///     A piece of middleware to add several headers to API responses.
/// </summary>
public class ApiHeadersMiddleware
{
    private readonly IHttpContextService _httpContextService;
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Constructs an <see cref="ApiHeadersMiddleware" /> instance.
    /// </summary>
    public ApiHeadersMiddleware(RequestDelegate next, IHttpContextService httpContextService)
    {
        _next = next;
        _httpContextService = httpContextService;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        if (_httpContextService.Path.Contains("ph-api/"))
        {
            _httpContextService.AddHeader("Access-Control-Allow-Origin", "*");
            _httpContextService.AddHeader("Access-Control-Allow-Headers", "Authorization, Content-Type");
            _httpContextService.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            _httpContextService.AddHeader("Cache-Control", "no-store, no-cache");
            _httpContextService.AddHeader("Expires", "-1");
            if (_httpContextService.GetHeaders().ContainsKey("Origin") && _httpContextService.Method.Equals("OPTIONS"))
            {
                _httpContextService.SetStatusCode(HttpStatusCode.OK);
            }
            else
            {
                await _next(context);
            }
        }
        else
        {
            await _next(context);
        }
    }
}
