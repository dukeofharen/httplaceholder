using System.Net;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Resources;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Commands;
using HttPlaceholder.Domain;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

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

    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextService _httpContextService;
    private readonly ILogger<StubHandlingMiddleware> _logger;
    private readonly IMediator _mediator;
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<SettingsModel> _options;
    private readonly IRequestLoggerFactory _requestLoggerFactory;
    private readonly IResourcesService _resourcesService;
    private readonly IStubContext _stubContext;
    private readonly IUrlResolver _urlResolver;


    /// <summary>
    ///     Constructs a <see cref="StubHandlingMiddleware" /> instance.
    /// </summary>
    public StubHandlingMiddleware(
        RequestDelegate next,
        IMediator mediator,
        IRequestLoggerFactory requestLoggerFactory,
        IResourcesService resourcesService,
        IStubContext stubContext,
        ILogger<StubHandlingMiddleware> logger,
        IClientDataResolver clientDataResolver,
        IOptionsMonitor<SettingsModel> options,
        IHttpContextService httpContextService,
        IUrlResolver urlResolver)
    {
        _next = next;
        _mediator = mediator;
        _requestLoggerFactory = requestLoggerFactory;
        _resourcesService = resourcesService;
        _stubContext = stubContext;
        _logger = logger;
        _clientDataResolver = clientDataResolver;
        _httpContextService = httpContextService;
        _urlResolver = urlResolver;
        _options = options;
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

        var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
        var settings = _options.CurrentValue;
        if (settings?.Stub?.HealthcheckOnRootUrl == true && _httpContextService.Path == "/")
        {
            _httpContextService.SetStatusCode(HttpStatusCode.OK);
            await _httpContextService.WriteAsync("OK", cancellationToken);
            return;
        }

        var correlationId = Guid.NewGuid().ToString();
        var requestLogger = _requestLoggerFactory.GetRequestLogger();
        requestLogger.SetCorrelationId(correlationId);
        ResponseModel response = null;
        try
        {
            response = await HandleRequest(correlationId, cancellationToken);
        }
        catch (RequestValidationException e)
        {
            await HandleRequestValidationException(correlationId, e, settings, cancellationToken);
        }
        catch (TaskCanceledException e)
        {
            _logger.LogDebug(e, "Request was cancelled.");
        }
        catch (Exception e)
        {
            HandleException(correlationId, e);
        }

        var loggingResult = requestLogger.GetResult();
        var enableRequestLogging = settings?.Storage?.EnableRequestLogging ?? false;
        if (enableRequestLogging)
        {
            _logger.LogInformation($"Request: {JObject.FromObject(loggingResult)}");
        }

        await _stubContext.AddRequestResultAsync(loggingResult, response, cancellationToken);
    }

    private void HandleException(string correlationId, Exception e)
    {
        _logger.LogWarning($"Unexpected exception thrown: {e}");
        _httpContextService.SetStatusCode(HttpStatusCode.InternalServerError);
        _httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlationId);
    }

    private async Task HandleRequestValidationException(string correlation, RequestValidationException e,
        SettingsModel settings, CancellationToken cancellationToken)
    {
        _httpContextService.SetStatusCode(HttpStatusCode.NotImplemented);
        _httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlation);
        if (settings?.Gui?.EnableUserInterface == true)
        {
            var pageContents = _resourcesService.ReadAsString("Files/StubNotConfigured.html")
                .Replace("[ROOT_URL]", _urlResolver.GetRootUrl());
            _httpContextService.AddHeader(HeaderKeys.ContentType, MimeTypes.HtmlMime);
            await _httpContextService.WriteAsync(pageContents, cancellationToken);
        }

        _logger.LogDebug($"Request validation exception thrown: {e.Message}");
    }

    private async Task<ResponseModel> HandleRequest(string correlation, CancellationToken cancellationToken)
    {
        var requestLogger = _requestLoggerFactory.GetRequestLogger();
        // Enable rewind here to be able to read the posted body multiple times.
        _httpContextService.EnableRewind();

        // Log the request here.
        requestLogger.LogRequestParameters(
            _httpContextService.Method,
            _urlResolver.GetDisplayUrl(),
            await _httpContextService.GetBodyAsBytesAsync(cancellationToken),
            _clientDataResolver.GetClientIp(),
            _httpContextService.GetHeaders());

        _httpContextService.ClearResponse();
        var response = await _mediator.Send(new HandleStubRequestCommand(), cancellationToken);
        if (response.AbortConnection)
        {
            _httpContextService.AbortConnection();
            return response;
        }

        _httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlation);
        var requestResult = requestLogger.GetResult();
        if (!string.IsNullOrWhiteSpace(requestResult.ExecutingStubId))
        {
            _httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderExecutedStub, requestResult.ExecutingStubId);
        }

        _httpContextService.SetStatusCode(response.StatusCode);
        foreach (var (key, value) in response.Headers)
        {
            _httpContextService.AddHeader(key, value);
        }

        if (response.Body != null && response.Body.Any())
        {
            await _httpContextService.WriteAsync(response.Body, cancellationToken);
        }

        return response;
    }
}
