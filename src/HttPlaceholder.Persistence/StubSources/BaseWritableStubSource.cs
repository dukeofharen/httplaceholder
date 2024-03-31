using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Persistence;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Persistence.StubSources;

/// <summary>
///     An abstract class that is used to implement a stub source types that also writes its data to a data source.
/// </summary>
public abstract class BaseWritableStubSource : IWritableStubSource
{
    /// <inheritdoc />
    public abstract Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>>
        GetStubsOverviewAsync(string distributionKey = null,
            CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(string stubId,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task PrepareStubSourceAsync(CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<RequestOverviewModel>> GetRequestResultsOverviewAsync(PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetRequestResultsAsync(pagingModel, distributionKey, cancellationToken))
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

    /// <inheritdoc />
    public abstract Task AddStubAsync(StubModel stub, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<bool> DeleteStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<RequestResultModel> GetRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task DeleteAllRequestResultsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<bool> DeleteRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task CleanOldRequestResultsAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<ScenarioStateModel> GetScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<ScenarioStateModel> AddScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task UpdateScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<IEnumerable<ScenarioStateModel>> GetAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<bool> DeleteScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task DeleteAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default);
}
