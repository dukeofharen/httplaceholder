using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;

namespace HttPlaceholder.Web.Shared.Middleware;

/// <summary>
///     A piece of middleware that is used in development scenarios; should NEVER be used in production scenarios.
///     Is only active when the ASPNETCORE_ENVIRONMENT environment variable is set to "Development".
/// </summary>
public class DevelopmentMiddleware
{
    private readonly RequestDelegate _next;

    /// <summary>
    ///     Constructs a <see cref="DevelopmentMiddleware" /> instance.
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

        var distKeyHeader = httpContextService.GetHeaders().FirstOrDefault(h =>
            h.Key.Equals("x-httplaceholder-distkey", StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(distKeyHeader.Value))
        {
            stubRequestContext.DistributionKey = distKeyHeader.Value;
        }

        await _next(context);
    }
}
