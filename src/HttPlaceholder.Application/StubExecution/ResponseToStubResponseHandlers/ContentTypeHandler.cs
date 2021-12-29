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
        var headerValue = response.Headers.CaseInsensitiveSearch("content-type");
        if (string.IsNullOrWhiteSpace(headerValue))
        {
            return Task.FromResult(false);
        }

        stubResponseModel.ContentType = headerValue;
        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => -1;
}
