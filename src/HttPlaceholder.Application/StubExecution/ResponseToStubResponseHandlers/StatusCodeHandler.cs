using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
///     This handler is used for setting the HTTP status code.
/// </summary>
internal class StatusCodeHandler : IResponseToStubResponseHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel,
        CancellationToken cancellationToken)
    {
        if (response.StatusCode <= 0)
        {
            return Task.FromResult(false);
        }

        stubResponseModel.StatusCode = response.StatusCode;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
