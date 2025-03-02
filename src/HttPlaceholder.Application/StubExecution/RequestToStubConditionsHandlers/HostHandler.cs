using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     "Request to stub conditions handler" that is used to create a hostname header condition.
/// </summary>
internal class HostHandler : IRequestToStubConditionsHandler, ISingletonService
{
    private static readonly int[] _defaultPorts = [80, 443];

    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        var uri = new Uri(request.Url);
        var host = uri.Host;
        if (!_defaultPorts.Contains(uri.Port))
        {
            host += $":{uri.Port}";
        }

        conditions.Host = new StubConditionStringCheckingModel { StringEquals = host };
        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => 0;
}
