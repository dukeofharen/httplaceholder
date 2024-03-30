using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
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
            return false.AsTask();
        }

        stubResponseModel.StatusCode = response.StatusCode;
        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => 0;
}
