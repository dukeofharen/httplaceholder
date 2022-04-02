using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Persistence;

/// <summary>
/// Describes a class that is used to implement a stub source types that also writes its data to a data source.
/// </summary>
public interface IWritableStubSource : IStubSource
{
    /// <summary>
    /// Adds a <see cref="StubModel"/>.
    /// </summary>
    /// <param name="stub">The stub.</param>
    Task AddStubAsync(StubModel stub);

    /// <summary>
    /// Deletes a stub.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    /// <returns>True if the stub was deleted, false otherwise.</returns>
    Task<bool> DeleteStubAsync(string stubId);

    /// <summary>
    /// Add a <see cref="RequestResultModel"/>.
    /// </summary>
    /// <param name="requestResult">The request.</param>
    /// /// <param name="responseModel">The response that belongs to the request.</param>
    Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel);

    /// <summary>
    /// Gets a list of <see cref="RequestResultModel"/>.
    /// </summary>
    /// <returns>A list of requests.</returns>
    Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();

    /// <summary>
    /// Gets a list of <see cref="RequestOverviewModel"/>.
    /// </summary>
    /// <returns>A list of overview requests.</returns>
    Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync();

    /// <summary>
    /// Gets a <see cref="RequestResultModel"/> by correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <returns>The request.</returns>
    Task<RequestResultModel> GetRequestAsync(string correlationId);

    /// <summary>
    /// Gets a <see cref="ResponseModel"/> by request correlation ID.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <returns>The response.</returns>
    Task<ResponseModel> GetResponseAsync(string correlationId);

    /// <summary>
    /// Deletes all requests.
    /// </summary>
    Task DeleteAllRequestResultsAsync();

    /// <summary>
    /// Deletes a specific request.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    /// <returns>True if the request was deleted, false otherwise.</returns>
    Task<bool> DeleteRequestAsync(string correlationId);

    /// <summary>
    /// Clean all old requests.
    /// </summary>
    Task CleanOldRequestResultsAsync();
}
