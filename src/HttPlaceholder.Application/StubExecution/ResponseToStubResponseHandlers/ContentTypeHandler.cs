using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;

/// <summary>
///     This handler is being used for setting the content type.
/// </summary>
internal class ContentTypeHandler : IResponseToStubResponseHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpResponseModel response, StubResponseModel stubResponseModel,
        CancellationToken cancellationToken)
    {
        var header = response.Headers.CaseInsensitiveSearchPair(HeaderKeys.ContentType);
        if (string.IsNullOrWhiteSpace(header.Value))
        {
            return false.AsTask();
        }

        stubResponseModel.ContentType = header.Value;
        response.Headers.Remove(header);
        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => -1;
}
