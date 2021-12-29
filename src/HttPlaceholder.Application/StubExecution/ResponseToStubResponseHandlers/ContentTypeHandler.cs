using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
/// This handler is being used for setting the content type.
/// </summary>
public class ContentTypeHandler : IResponseToStubResponseHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel)
    {
        var header = response.Headers.CaseInsensitiveSearchPair("content-type");
        if (string.IsNullOrWhiteSpace(header.Value))
        {
            return Task.FromResult(false);
        }

        stubResponseModel.ContentType = header.Value;
        response.Headers.Remove(header);
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => -1;
}
