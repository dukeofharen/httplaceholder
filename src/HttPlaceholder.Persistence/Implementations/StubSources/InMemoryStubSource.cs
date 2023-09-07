using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to store and read data from memory.
/// </summary>
internal class InMemoryStubSource : BaseWritableStubSource
{
    private static readonly object _lock = new();

    private readonly IOptionsMonitor<SettingsModel> _options;

    internal readonly IDictionary<RequestResultModel, ResponseModel> RequestResponseMap =
        new Dictionary<RequestResultModel, ResponseModel>();

    internal readonly IList<RequestResultModel> RequestResultModels = new List<RequestResultModel>();
    internal readonly IList<StubModel> StubModels = new List<StubModel>();
    internal readonly IList<ResponseModel> StubResponses = new List<ResponseModel>();

    public InMemoryStubSource(IOptionsMonitor<SettingsModel> options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public override Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (responseModel != null)
            {
                requestResult.HasResponse = true;
                StubResponses.Add(responseModel);
                RequestResponseMap.Add(requestResult, responseModel);
            }

            RequestResultModels.Add(requestResult);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task AddStubAsync(StubModel stub, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            StubModels.Add(stub);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task<RequestResultModel> GetRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(GetRequest(correlationId));
        }
    }

    /// <inheritdoc />
    public override Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var nullValue = Task.FromResult((ResponseModel)null);
            var request = GetRequest(correlationId);
            if (request == null)
            {
                return nullValue;
            }

            return !RequestResponseMap.ContainsKey(request) ? nullValue : Task.FromResult(RequestResponseMap[request]);
        }
    }

    /// <inheritdoc />
    public override Task DeleteAllRequestResultsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            RequestResultModels.Clear();
            StubResponses.Clear();
            RequestResponseMap.Clear();
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task<bool> DeleteRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var request = RequestResultModels.FirstOrDefault(r => r.CorrelationId == correlationId);
            if (request == null)
            {
                return Task.FromResult(false);
            }

            RequestResultModels.Remove(request);
            RemoveResponse(request);
            return Task.FromResult(true);
        }
    }

    /// <inheritdoc />
    public override Task<bool> DeleteStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var stub = StubModels.FirstOrDefault(s => s.Id == stubId);
            if (stub == null)
            {
                return Task.FromResult(false);
            }

            StubModels.Remove(stub);
            return Task.FromResult(true);
        }
    }

    /// <inheritdoc />
    public override Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var result = RequestResultModels.OrderByDescending(r => r.RequestBeginTime).ToArray();
            if (pagingModel != null)
            {
                IEnumerable<RequestResultModel> resultQuery = result;
                if (!string.IsNullOrWhiteSpace(pagingModel.FromIdentifier))
                {
                    var index = result
                        .Select((request, index) => new {request, index})
                        .Where(f => f.request.CorrelationId.Equals(pagingModel.FromIdentifier))
                        .Select(f => f.index)
                        .FirstOrDefault();
                    resultQuery = result
                        .Skip(index);
                }

                if (pagingModel.ItemsPerPage.HasValue)
                {
                    resultQuery = resultQuery.Take(pagingModel.ItemsPerPage.Value);
                }

                result = resultQuery.ToArray();
            }

            return Task.FromResult(result.AsEnumerable());
        }
    }

    /// <inheritdoc />
    public override Task<IEnumerable<StubModel>> GetStubsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // We need to convert the list to an array here, or else we can get errors when deleting the stubs.
            return Task.FromResult(StubModels.ToArray().AsEnumerable());
        }
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<StubOverviewModel>> GetStubsOverviewAsync(string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => new StubOverviewModel {Id = s.Id, Tenant = s.Tenant, Enabled = s.Enabled})
        .ToArray();

    /// <inheritdoc />
    public override Task<StubModel> GetStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(StubModels.FirstOrDefault(s => s.Id == stubId));

    /// <inheritdoc />
    public override Task CleanOldRequestResultsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var maxLength = _options.CurrentValue.Storage?.OldRequestsQueueLength ?? 40;
            var requests = RequestResultModels
                .OrderByDescending(r => r.RequestEndTime)
                .Skip(maxLength);
            foreach (var request in requests)
            {
                RequestResultModels.Remove(request);
                RemoveResponse(request);
            }

            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task PrepareStubSourceAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void RemoveResponse(RequestResultModel request)
    {
        if (!RequestResponseMap.ContainsKey(request))
        {
            return;
        }

        var response = RequestResponseMap[request];
        StubResponses.Remove(response);
        RequestResponseMap.Remove(request);
    }

    private RequestResultModel GetRequest(string correlationId) =>
        RequestResultModels.FirstOrDefault(r => r.CorrelationId == correlationId);
}
