using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution;

/// <summary>
///     Describes a class that is used to convert a <see cref="HttpResponseModel" /> to <see cref="StubResponseModel" />.
///     To perform the mapping, the <see cref="IResponseToStubResponseHandler" /> implementations are used.
/// </summary>
public interface IHttpResponseToStubResponseService
{
    /// <summary>
    ///     Converts a <see cref="HttpResponseModel" /> to <see cref="StubResponseModel" />.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The <see cref="StubResponseModel" />.</returns>
    Task<StubResponseModel> ConvertToResponseAsync(HttpResponseModel response, CancellationToken cancellationToken);
}
