using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.Interfaces.Resources;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class StubHandler : IStubHandler, ISingletonService
{
    private readonly IRequestLoggerFactory _requestLoggerFactory;
    private readonly IResourcesService _resourcesService;
    private readonly IStubContext _stubContext;
    private readonly IStubRequestExecutor _stubRequestExecutor;
    private readonly ILogger<StubHandler> _logger;
    private readonly IClientDataResolver _clientDataResolver;
    private readonly IHttpContextService _httpContextService;
    private readonly SettingsModel _settings;

    /// <summary>
    ///     Constructs a <see cref="StubHandler"/> instance
    /// </summary>
    public StubHandler(
        IRequestLoggerFactory requestLoggerFactory,
        IResourcesService resourcesService,
        IStubContext stubContext,
        IStubRequestExecutor stubRequestExecutor,
        ILogger<StubHandler> logger,
        IClientDataResolver clientDataResolver,
        IOptionsMonitor<SettingsModel> options,
        IHttpContextService httpContextService)
    {
        _requestLoggerFactory = requestLoggerFactory;
        _resourcesService = resourcesService;
        _stubContext = stubContext;
        _stubRequestExecutor = stubRequestExecutor;
        _logger = logger;
        _clientDataResolver = clientDataResolver;
        _httpContextService = httpContextService;
        _settings = options.CurrentValue;
    }

    /// <inheritdoc />
    public async Task HandleStubRequestAsync(CancellationToken cancellationToken)
    {
        if (_settings?.Stub?.HealthcheckOnRootUrl == true && _httpContextService.Path == "/")
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
            await HandleRequestValidationException(correlationId, e, _settings, cancellationToken);
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
                .Replace("[ROOT_URL]", _httpContextService.RootUrl);
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
            _httpContextService.DisplayUrl,
            await _httpContextService.GetBodyAsBytesAsync(cancellationToken),
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

        if (response.Body != null && response.Body.Any())
        {
            await _httpContextService.WriteAsync(response.Body, cancellationToken);
        }

        return response;
    }
}
