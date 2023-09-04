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
    /// <param name="distributionKey">The distribution key the stubs should be added for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddStubAsync(StubModel stub, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a stub.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <param name="distributionKey">The distribution key the stubs should be deleted for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the stub was deleted, false otherwise.</returns>
    Task<bool> DeleteStubAsync(string stubId, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Add a <see cref="RequestResultModel" />.
    /// </summary>
    /// <param name="requestResult">The request.</param>
    /// <param name="responseModel">The response that belongs to the request.</param>
    /// <param name="distributionKey">The distribution key the request should be added for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task AddRequestResultAsync(
        RequestResultModel requestResult,
        ResponseModel responseModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of <see cref="RequestResultModel" />.
    /// </summary>
    /// <param name="pagingModel">The paging information.</param>
    /// <param name="distributionKey">The distribution key the requests should be retrieved for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of requests.</returns>
    Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(
        PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a list of <see cref="RequestOverviewModel" />.
    /// </summary>
    /// <param name="pagingModel">The paging information.</param>
    /// <param name="distributionKey">The distribution key the requests should be retrieved for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of overview requests.</returns>
    Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(
        PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a <see cref="RequestResultModel" /> by correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="distributionKey">The distribution key the request should be retrieved for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The request.</returns>
    Task<RequestResultModel> GetRequestAsync(string correlationId, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a <see cref="ResponseModel" /> by request correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="distributionKey">The distribution key the response should be retrieved for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response.</returns>
    Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes all requests.
    /// </summary>
    /// <param name="distributionKey">The distribution key the requests should be deleted for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DeleteAllRequestResultsAsync(string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a specific request.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <param name="distributionKey">The distribution key the request should be deleted for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the request was deleted, false otherwise.</returns>
    Task<bool> DeleteRequestAsync(string correlationId, string distributionKey = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Clean all old requests.
    /// </summary>
    /// <param name="distributionKey">The distribution key the requests should be cleaned for. Leave it null if there is no user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task CleanOldRequestResultsAsync(string distributionKey = null, CancellationToken cancellationToken = default);
}
