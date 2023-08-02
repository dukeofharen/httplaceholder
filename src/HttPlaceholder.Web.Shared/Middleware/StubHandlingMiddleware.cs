using System.Net;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Web.Shared.Middleware;

/// <summary>
///     A piece of middleware for matching requests against stubs.
/// </summary>
public class StubHandlingMiddleware
{
    private static readonly string[] _segmentsToIgnore =
    {
        "/ph-api", "/ph-ui", "/ph-static", "swagger", "/requestHub", "/scenarioHub", "/stubHub"
    };

    private readonly IHttpContextService _httpContextService;
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<SettingsModel> _options;
    private readonly IStubHandler _stubHandler;


    /// <summary>
    ///     Constructs a <see cref="StubHandlingMiddleware" /> instance.
    /// </summary>
    public StubHandlingMiddleware(
        RequestDelegate next,
        IHttpContextService httpContextService,
        IOptionsMonitor<SettingsModel> options,
        IStubHandler stubHandler)
    {
        _next = next;
        _httpContextService = httpContextService;
        _options = options;
        _stubHandler = stubHandler;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
        var path = _httpContextService.Path;
        var settings = _options.CurrentValue;
        if (settings?.Stub?.HealthcheckOnRootUrl == true && path == "/")
        {
            _httpContextService.SetStatusCode(HttpStatusCode.OK);
            await _httpContextService.WriteAsync("OK", cancellationToken);
            return;
        }

        if (_segmentsToIgnore.Any(s => path.Contains(s, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        await _stubHandler.HandleStubRequestAsync(cancellationToken);
    }
}
