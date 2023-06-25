using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Persistence;

/// <summary>
///     Describes a class that is used to implement a stub source types that also writes its data to a data source.
/// </summary>
public interface IWritableStubSource : IStubSource
{
    /// <summary>
    ///     Adds a <see cref="StubModel" />.
    /// </summary>
    /// <param name="stub">The stub.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddStubAsync(StubModel stub, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a stub.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the stub was deleted, false otherwise.</returns>
    Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken);

    /// <summary>
    ///     Add a <see cref="RequestResultModel" />.
    /// </summary>
    /// <param name="requestResult">The request.</param>
    /// <param name="responseModel">The response that belongs to the request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Gets a list of <see cref="RequestResultModel" />.
    /// </summary>
    /// <param name="pagingModel">The paging information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of requests.</returns>
    Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(PagingModel pagingModel, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets a list of <see cref="RequestOverviewModel" />.
    /// </summary>
    /// <param name="pagingModel">The paging information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of overview requests.</returns>
    Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(PagingModel pagingModel, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets a <see cref="RequestResultModel" /> by correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request.</returns>
    Task<RequestResultModel> GetRequestAsync(string correlationId, CancellationToken cancellationToken);

    /// <summary>
    ///     Gets a <see cref="ResponseModel" /> by request correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    Task<ResponseModel> GetResponseAsync(string correlationId, CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes all requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes a specific request.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the request was deleted, false otherwise.</returns>
    Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken);

    /// <summary>
    ///     Clean all old requests.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CleanOldRequestResultsAsync(CancellationToken cancellationToken);
}
