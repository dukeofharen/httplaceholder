using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Commands.HandleStubRequest;
using MediatR;

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
    private readonly IMediator _mediator;


    /// <summary>
    ///     Constructs a <see cref="StubHandlingMiddleware" /> instance.
    /// </summary>
    public StubHandlingMiddleware(
        RequestDelegate next,
        IHttpContextService httpContextService,
        IMediator mediator)
    {
        _next = next;
        _httpContextService = httpContextService;
        _mediator = mediator;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(
        HttpContext context)
    {
        if (_segmentsToIgnore.Any(s => _httpContextService.Path.Contains(s, StringComparison.OrdinalIgnoreCase)))
        {
            await _next(context);
            return;
        }

        await _mediator.Send(new HandleStubRequestCommand(), context?.RequestAborted ?? CancellationToken.None);
    }
}
