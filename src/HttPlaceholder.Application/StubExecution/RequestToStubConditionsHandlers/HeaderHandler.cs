using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     "Request to stub conditions handler" that is used to request header conditions.
/// </summary>
internal class HeaderHandler : IRequestToStubConditionsHandler, ISingletonService
{
    private static readonly IEnumerable<string> _headersToStrip = new[] {"Postman-Token", "Host"};

    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        if (!request.Headers.Any())
        {
            return Task.FromResult(false);
        }

        // Do a Regex escape here, if we don do this it might give some strange results later on
        // and filter some headers out.
        conditions.Headers = request.Headers
            .Where(h => !_headersToStrip.Contains(h.Key, StringComparer.OrdinalIgnoreCase))
            .ToDictionary(d => d.Key,
                d => new StubConditionStringCheckingModel {StringEquals = d.Value} as object);
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 1;
}
