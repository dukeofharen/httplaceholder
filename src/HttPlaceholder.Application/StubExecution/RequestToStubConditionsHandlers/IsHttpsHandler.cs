using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
/// "Request to stub conditions handler" that is used to create an HTTP/HTTPS checking condition.
/// </summary>
internal class IsHttpsHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        if (!request.Url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(false);
        }

        conditions.Url.IsHttps = true;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
