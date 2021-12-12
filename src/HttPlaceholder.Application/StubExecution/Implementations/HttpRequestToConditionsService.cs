using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc/>
internal class HttpRequestToConditionsService : IHttpRequestToConditionsService
{
    private readonly IEnumerable<IRequestToStubConditionsHandler> _handlers;
    private readonly ILogger<HttpRequestToConditionsService> _logger;

    public HttpRequestToConditionsService(
        IEnumerable<IRequestToStubConditionsHandler> handlers,
        ILogger<HttpRequestToConditionsService> logger)
    {
        _handlers = handlers;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<StubConditionsModel> ConvertToConditionsAsync(HttpRequestModel request)
    {
        var conditions = new StubConditionsModel();
        foreach (var handler in _handlers.OrderByDescending(w => w.Priority))
        {
            var executed =
                await handler.HandleStubGenerationAsync(request, conditions);
            _logger.LogDebug(
                $"Handler '{handler.GetType().Name}' " + (executed ? "executed" : "not executed") + ".");
        }

        return conditions;
    }
}