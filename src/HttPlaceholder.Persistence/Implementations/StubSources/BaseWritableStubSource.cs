using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     An abstract class that is used to implement a stub source types that also writes its data to a data source.
/// </summary>
public abstract class BaseWritableStubSource : IWritableStubSource
{
    /// <inheritdoc />
    public abstract Task<IEnumerable<StubModel>> GetStubsAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<StubModel> GetStubAsync(string stubId, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task PrepareStubSourceAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task AddStubAsync(StubModel stub, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<bool> DeleteStubAsync(string stubId, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(PagingModel pagingModel,
        CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<RequestResultModel> GetRequestAsync(string correlationId, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<ResponseModel> GetResponseAsync(string correlationId, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task DeleteAllRequestResultsAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<bool> DeleteRequestAsync(string correlationId, CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task CleanOldRequestResultsAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(
        PagingModel pagingModel,
        CancellationToken cancellationToken) =>
        (await GetRequestResultsAsync(pagingModel, cancellationToken))
        .Select(r => new RequestOverviewModel
        {
            Method = r.RequestParameters?.Method,
            Url = r.RequestParameters?.Url,
            CorrelationId = r.CorrelationId,
            StubTenant = r.StubTenant,
            ExecutingStubId = r.ExecutingStubId,
            RequestBeginTime = r.RequestBeginTime,
            RequestEndTime = r.RequestEndTime,
            HasResponse = r.HasResponse
        });
}
