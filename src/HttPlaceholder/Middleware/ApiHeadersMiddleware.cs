using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using Microsoft.AspNetCore.Http;

namespace HttPlaceholder.Middleware;

public class ApiHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpContextService _httpContextService;

    public ApiHeadersMiddleware(RequestDelegate next, IHttpContextService httpContextService)
    {
        _next = next;
        _httpContextService = httpContextService;
    }

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
