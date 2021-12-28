using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
/// This handler is being used for setting the content type.
/// </summary>
public class ContentTypeHandler : IResponseToStubResponseHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel) => throw new System.NotImplementedException();

    /// <inheritdoc />
    public int Priority => -1;
}
