using System.Net;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Web.Shared.Middleware;

/// <summary>
///     A piece of middleware that is used in development scenarios; should NEVER be used in production scenarios.
///     Is only active when the ASPNETCORE_ENVIRONMENT environment variable is set to "Development".
/// </summary>
public class DevelopmentMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Constructs a <see cref="DevelopmentMiddleware"/> instance.
    /// </summary>
    public DevelopmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(
        HttpContext context,
        IHttpContextService httpContextService,
        IStubRequestContext stubRequestContext,
        IEnvService envService)
    {
        var env = envService.GetAspNetCoreEnvironment();
        if (string.IsNullOrWhiteSpace(env) || !env.Equals("development", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var path = httpContextService.Path;
        if (!path.StartsWith("/ph-development"))
        {
            await _next(context);
            return;
        }

        var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
        var method = httpContextService.Method;
        if (method.Equals("POST", StringComparison.OrdinalIgnoreCase) &&
            path.EndsWith("/set-distribution-key", StringComparison.OrdinalIgnoreCase))
        {
            // Set the distribution key globally for dev purposes.
            var body = await httpContextService.GetBodyAsync(cancellationToken);
            var json = JObject.Parse(body);
            var key = json.SelectToken("$.key")?.ToObject<string>();
            if (key != null)
            {
                stubRequestContext.DistributionKey = key;
                httpContextService.SetStatusCode(HttpStatusCode.NoContent);
            }
            else
            {
                httpContextService.SetStatusCode(HttpStatusCode.BadRequest);
            }
        }
    }
}
