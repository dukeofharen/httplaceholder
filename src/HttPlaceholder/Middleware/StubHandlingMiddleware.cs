using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Resources;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Middleware;

/// <summary>
///     A piece of middleware for matching requests against stubs.
/// </summary>
public class StubHandlingMiddleware
{
    private static readonly string[] _segmentsToIgnore =
    {
        "/ph-api", "/ph-ui", "/ph-static", "swagger", "/requestHub", "/scenarioHub"
    };

    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextService _httpContextService;
    private readonly ILogger<StubHandlingMiddleware> _logger;
    private readonly RequestDelegate _next;
    private readonly IRequestLoggerFactory _requestLoggerFactory;
    private readonly IResourcesService _resourcesService;
    private readonly SettingsModel _settings;
    private readonly IStubContext _stubContext;
    private readonly IStubRequestExecutor _stubRequestExecutor;

    /// <summary>
    ///     Constructs a <see cref="StubHandlingMiddleware" /> instance.
    /// </summary>
    public StubHandlingMiddleware(
        RequestDelegate next,
        IClientDataResolver clientDataResolver,
        IHttpContextService httpContextService,
        ILogger<StubHandlingMiddleware> logger,
        IRequestLoggerFactory requestLoggerFactory,
        IStubContext stubContext,
        IStubRequestExecutor stubRequestExecutor,
        IOptions<SettingsModel> options,
        IResourcesService resourcesService)
    {
        _next = next;
        _clientDataResolver = clientDataResolver;
        _httpContextService = httpContextService;
        _logger = logger;
        _requestLoggerFactory = requestLoggerFactory;
        _stubContext = stubContext;
        _stubRequestExecutor = stubRequestExecutor;
        _resourcesService = resourcesService;
        _settings = options.Value;
    }

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
        var path = _httpContextService.Path;
        if (_settings?.Stub?.HealthcheckOnRootUrl == true && path == "/")
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
            await HandleRequestValidationException(correlationId, e, cancellationToken);
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
        var enableRequestLogging = _settings?.Storage?.EnableRequestLogging ?? false;
        if (enableRequestLogging)
        {
            _logger.LogInformation($"Request: {JObject.FromObject(loggingResult)}");
        }

        await _stubContext.AddRequestResultAsync(loggingResult, response, cancellationToken);
    }

    private void HandleException(string correlationId, Exception e)
    {
        _httpContextService.SetStatusCode(HttpStatusCode.InternalServerError);
        _httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlationId);
        _logger.LogWarning($"Unexpected exception thrown: {e}");
    }

    private async Task HandleRequestValidationException(string correlation, RequestValidationException e,
        CancellationToken cancellationToken)
    {
        _httpContextService.SetStatusCode(HttpStatusCode.NotImplemented);
        _httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlation);
        if (_settings?.Gui?.EnableUserInterface == true)
        {
            var pageContents = _resourcesService.ReadAsString("Files/StubNotConfigured.html")
                .Replace("[ROOT_URL]", _httpContextService.RootUrl);
            _httpContextService.AddHeader(HeaderKeys.ContentType, Constants.HtmlMime);
            await _httpContextService.WriteAsync(pageContents, cancellationToken);
        }

        _logger.LogDebug($"Request validation exception thrown: {e.Message}");
    }

    private async Task<ResponseModel> HandleRequest(string correlation, CancellationToken cancellationToken)
    {
        var requestLogger = _requestLoggerFactory.GetRequestLogger();
        // Enable rewind here to be able to read the posted body multiple times.
        _httpContextService.EnableRewind();

        // Log the request here
        requestLogger.LogRequestParameters(
            _httpContextService.Method,
            _httpContextService.DisplayUrl,
            _httpContextService.GetBody(),
            _clientDataResolver.GetClientIp(),
            _httpContextService.GetHeaders());

        _httpContextService.ClearResponse();
        var response = await _stubRequestExecutor.ExecuteRequestAsync(cancellationToken);
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

        if (response.Body != null)
        {
            await _httpContextService.WriteAsync(response.Body, cancellationToken);
        }

        return response;
    }
}
