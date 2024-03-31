using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
///     This handler is used for setting the stub response headers.
/// </summary>
internal class HeaderHandler : IResponseToStubResponseHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel,
        CancellationToken cancellationToken)
    {
        stubResponseModel.Headers = response.Headers;
        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => 0;
}
