using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
/// This handler is used for setting the HTTP status code.
/// </summary>
public class StatusCodeHandler : IResponseToStubResponseHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public int Priority => 0;
}
