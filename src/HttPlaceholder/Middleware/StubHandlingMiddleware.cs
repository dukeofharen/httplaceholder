using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Signalling;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using HttPlaceholder.Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Middleware;

/// <summary>
/// A piece of middleware for matching requests against stubs.
/// </summary>
public class StubHandlingMiddleware
{
    private static readonly string[] _segmentsToIgnore = {"/ph-api", "/ph-ui", "/ph-static", "swagger", "/requestHub", "/scenarioHub"};

    private const string CorrelationHeaderKey = "X-HttPlaceholder-Correlation";
    private const string ExecutedStubHeaderKey = "X-HttPlaceholder-ExecutedStub";
    private readonly RequestDelegate _next;
    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextService _httpContextService;
    private readonly ILogger<StubHandlingMiddleware> _logger;
    private readonly IRequestLoggerFactory _requestLoggerFactory;
    private readonly IStubContext _stubContext;
    private readonly IStubRequestExecutor _stubRequestExecutor;
    private readonly SettingsModel _settings;

    /// <summary>
    /// Constructs a <see cref="StubHandlingMiddleware"/> instance.
    /// </summary>
    public StubHandlingMiddleware(
        RequestDelegate next,
        IClientDataResolver clientDataResolver,
        IHttpContextService httpContextService,
        ILogger<StubHandlingMiddleware> logger,
        IRequestLoggerFactory requestLoggerFactory,
        IStubContext stubContext,
        IStubRequestExecutor stubRequestExecutor,
        IOptions<SettingsModel> options)
    {
        _next = next;
        _clientDataResolver = clientDataResolver;
        _httpContextService = httpContextService;
        _logger = logger;
        _requestLoggerFactory = requestLoggerFactory;
        _stubContext = stubContext;
        _stubRequestExecutor = stubRequestExecutor;
        _settings = options.Value;
    }

    /// <summary>
    /// Handles the middleware.
    /// </summary>
    public async Task Invoke(HttpContext context)
    {
        var path = _httpContextService.Path;
        if (_settings?.Stub?.HealthcheckOnRootUrl == true && path == "/")
        {
            _httpContextService.SetStatusCode(HttpStatusCode.OK);
            await _httpContextService.WriteAsync("OK");
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
            response = await HandleRequest(correlationId);
        }
        catch (RequestValidationException e)
        {
            await HandleRequestValidationException(correlationId, e);
        }
        catch (Exception e)
        {
            HandleException(correlationId, e);
        }

        var loggingResult = requestLogger.GetResult();
        if (!string.IsNullOrWhiteSpace(loggingResult.ExecutingStubId))
        {
            _httpContextService.TryAddHeader(ExecutedStubHeaderKey, loggingResult.ExecutingStubId);
        }

        var jsonLoggingResult = JObject.FromObject(loggingResult);
        var enableRequestLogging = _settings?.Storage?.EnableRequestLogging ?? false;
        if (enableRequestLogging)
        {
            _logger.LogInformation($"Request: {jsonLoggingResult}");
        }

        await _stubContext.AddRequestResultAsync(loggingResult, response);
    }

    private void HandleException(string correlationId, Exception e)
    {
        _httpContextService.SetStatusCode(HttpStatusCode.InternalServerError);
        _httpContextService.TryAddHeader(CorrelationHeaderKey, correlationId);
        _logger.LogWarning($"Unexpected exception thrown: {e}");
    }

    private async Task HandleRequestValidationException(string correlation, RequestValidationException e)
    {
        _httpContextService.SetStatusCode(HttpStatusCode.NotImplemented);
        _httpContextService.TryAddHeader(CorrelationHeaderKey, correlation);
        if (_settings?.Gui?.EnableUserInterface == true)
        {
            var pageContents =
                StaticResources.stub_not_configured_html_page.Replace("[ROOT_URL]", _httpContextService.RootUrl);
            _httpContextService.AddHeader("Content-Type", Constants.HtmlMime);
            await _httpContextService.WriteAsync(pageContents);
        }

        _logger.LogInformation($"Request validation exception thrown: {e.Message}");
    }

    private async Task<ResponseModel> HandleRequest(string correlation)
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
        var response = await _stubRequestExecutor.ExecuteRequestAsync();
        if (response.AbortConnection)
        {
            _httpContextService.AbortConnection();
            return response;
        }

        _httpContextService.TryAddHeader(CorrelationHeaderKey, correlation);
        _httpContextService.SetStatusCode(response.StatusCode);
        foreach (var (key, value) in response.Headers)
        {
            _httpContextService.AddHeader(key, value);
        }

        if (response.Body != null)
        {
            await _httpContextService.WriteAsync(response.Body);
        }

        return response;
    }
}
