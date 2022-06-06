using System;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
/// "Request to stub conditions handler" that is used to create a hostname header condition.
/// </summary>
internal class HostHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        var uri = new Uri(request.Url);
        var host = uri.Host;
        if (uri.Port != 80 && uri.Port != 443)
        {
            host += $":{uri.Port}";
        }

        conditions.Host = new StubConditionStringCheckingModel{StringEquals = host};
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
