using System.Net;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Application.StubExecution.Commands;
using HttPlaceholder.Domain;
using HttPlaceholder.Web.Shared.Resources;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Web.Shared.Middleware;

/// <summary>
///     A piece of middleware for matching requests against stubs.
/// </summary>
public class StubHandlingMiddleware(
    RequestDelegate next,
    IMediator mediator,
    IRequestLoggerFactory requestLoggerFactory,
    IStubContext stubContext,
    ILogger<StubHandlingMiddleware> logger,
    IClientDataResolver clientDataResolver,
    IOptionsMonitor<SettingsModel> options,
    IHttpContextService httpContextService,
    IUrlResolver urlResolver)
{
    private static readonly string[] _segmentsToIgnore =
    [
        "/ph-api", "/ph-ui", "/ph-static", "swagger", "/requestHub", "/scenarioHub", "/stubHub"
    ];

    /// <summary>
    ///     Handles the middleware.
    /// </summary>
    public async Task Invoke(
        HttpContext context)
    {
        if (_segmentsToIgnore.Any(s => httpContextService.Path.Contains(s, StringComparison.OrdinalIgnoreCase)))
        {
            await next(context);
            return;
        }

        var cancellationToken = context?.RequestAborted ?? CancellationToken.None;
        var settings = options.CurrentValue;
        if (settings?.Stub?.HealthcheckOnRootUrl == true && httpContextService.Path == "/")
        {
            httpContextService.SetStatusCode(HttpStatusCode.OK);
            await httpContextService.WriteAsync("OK", cancellationToken);
            return;
        }

        var correlationId = Guid.NewGuid().ToString();
        var requestLogger = requestLoggerFactory.GetRequestLogger();
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
            logger.LogDebug(e, "Request was cancelled.");
        }
        catch (Exception e)
        {
            HandleException(correlationId, e);
        }

        var loggingResult = requestLogger.GetResult();
        var enableRequestLogging = settings?.Storage?.EnableRequestLogging ?? false;
        if (enableRequestLogging)
        {
            logger.LogInformation("Request: {Result}", JObject.FromObject(loggingResult));
        }

        await stubContext.AddRequestResultAsync(loggingResult, response, cancellationToken);
    }

    private void HandleException(string correlationId, Exception e)
    {
        logger.LogWarning($"Unexpected exception thrown: {e}");
        httpContextService.SetStatusCode(HttpStatusCode.InternalServerError);
        httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlationId);
    }

    private async Task HandleRequestValidationException(string correlation, RequestValidationException e,
        SettingsModel settings, CancellationToken cancellationToken)
    {
        httpContextService.SetStatusCode(HttpStatusCode.NotImplemented);
        httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlation);
        if (settings?.Gui?.EnableUserInterface == true)
        {
            var pageContents = WebResources.StubNotConfiguredHtml
                .Replace("[ROOT_URL]", urlResolver.GetRootUrl());
            httpContextService.AddHeader(HeaderKeys.ContentType, MimeTypes.HtmlMime);
            await httpContextService.WriteAsync(pageContents, cancellationToken);
        }

        logger.LogDebug($"Request validation exception thrown: {e.Message}");
    }

    private async Task<ResponseModel> HandleRequest(string correlation, CancellationToken cancellationToken)
    {
        var requestLogger = requestLoggerFactory.GetRequestLogger();

        // Log the request here.
        requestLogger.LogRequestParameters(
            httpContextService.Method,
            urlResolver.GetDisplayUrl(),
            await httpContextService.GetBodyAsBytesAsync(cancellationToken),
            clientDataResolver.GetClientIp(),
            httpContextService.GetHeaders());

        httpContextService.ClearResponse();
        var response = await mediator.Send(new HandleStubRequestCommand(), cancellationToken);
        if (response.AbortConnection)
        {
            httpContextService.AbortConnection();
            return response;
        }

        httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderCorrelation, correlation);
        var requestResult = requestLogger.GetResult();
        if (!string.IsNullOrWhiteSpace(requestResult.ExecutingStubId))
        {
            httpContextService.TryAddHeader(HeaderKeys.XHttPlaceholderExecutedStub, requestResult.ExecutingStubId);
        }

        httpContextService.SetStatusCode(response.StatusCode);
        foreach (var (key, value) in response.Headers)
        {
            httpContextService.AddHeader(key, value);
        }

        if (response.Body != null && response.Body.Length != 0)
        {
            await httpContextService.WriteAsync(response.Body, cancellationToken);
        }

        return response;
    }
}
