using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <inheritdoc />
public class ClientIpHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        conditions.ClientIp = request.ClientIp;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}