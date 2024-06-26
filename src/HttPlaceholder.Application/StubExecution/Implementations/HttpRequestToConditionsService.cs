﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class HttpRequestToConditionsService(
    IEnumerable<IRequestToStubConditionsHandler> handlers,
    ILogger<HttpRequestToConditionsService> logger)
    : IHttpRequestToConditionsService, ISingletonService
{
    /// <inheritdoc />
    public async Task<StubConditionsModel> ConvertToConditionsAsync(HttpRequestModel request,
        CancellationToken cancellationToken)
    {
        var conditions = new StubConditionsModel();
        foreach (var handler in handlers.OrderByDescending(w => w.Priority))
        {
            var executed =
                await handler.HandleStubGenerationAsync(request, conditions, cancellationToken);
            logger.LogDebug(
                "Handler '{HandlerName}' execution result: {ExecutionResult}.",
                handler.GetType().Name,
                executed ? "executed" : "not executed");
        }

        return conditions;
    }
}
