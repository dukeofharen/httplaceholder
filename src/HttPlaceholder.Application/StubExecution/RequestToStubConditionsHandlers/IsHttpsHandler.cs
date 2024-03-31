using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     "Request to stub conditions handler" that is used to create an HTTP/HTTPS checking condition.
/// </summary>
internal class IsHttpsHandler : IRequestToStubConditionsHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        if (!request.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        {
            return false.AsTask();
        }

        conditions.Url.IsHttps = true;
        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => 0;
}
