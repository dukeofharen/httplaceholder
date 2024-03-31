using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.StubSources;

/// <summary>
///     A stub source that is used to store and read data from memory.
/// </summary>
internal class InMemoryStubSource(IOptionsMonitor<SettingsModel> options) : BaseWritableStubSource
{
    private static readonly object _lock = new();
    private readonly ConcurrentDictionary<string, StubRequestCollectionItem> _collectionItems = new();

    /// <inheritdoc />
    public override Task AddRequestResultAsync(RequestResultModel requestResult, ResponseModel responseModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var item = GetCollection(distributionKey);
            if (responseModel != null)
            {
                requestResult.HasResponse = true;
                item.StubResponses.Add(responseModel);
                item.RequestResponseMap.Add(requestResult, responseModel);
            }

            item.RequestResultModels.Add(requestResult);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task AddStubAsync(StubModel stub, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var item = GetCollection(distributionKey);
            item.StubModels.Add(stub);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task<RequestResultModel> GetRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return GetRequest(correlationId, distributionKey).AsTask();
        }
    }

    /// <inheritdoc />
    public override Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var nullValue = ((ResponseModel)null).AsTask();
            var request = GetRequest(correlationId, distributionKey);
            if (request == null)
            {
                return nullValue;
            }

            var item = GetCollection(distributionKey);
            return !item.RequestResponseMap.TryGetValue(request, out var value)
                ? nullValue
                : value.AsTask();
        }
    }

    /// <inheritdoc />
    public override Task DeleteAllRequestResultsAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var item = GetCollection(distributionKey);
            item.RequestResultModels.Clear();
            item.StubResponses.Clear();
            item.RequestResponseMap.Clear();
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task<bool> DeleteRequestAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var item = GetCollection(distributionKey);
            var request = item.RequestResultModels.FirstOrDefault(r => r.CorrelationId == correlationId);
            if (request == null)
            {
                return false.AsTask();
            }

            item.RequestResultModels.Remove(request);
            RemoveResponse(request, distributionKey);
            return true.AsTask();
        }
    }

    /// <inheritdoc />
    public override Task<bool> DeleteStubAsync(string stubId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var item = GetCollection(distributionKey);
            var stub = item.StubModels.FirstOrDefault(s => s.Id == stubId);
            if (stub == null)
            {
                return false.AsTask();
            }

            item.StubModels.Remove(stub);
            return true.AsTask();
        }
    }

    /// <inheritdoc />
    public override Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync(PagingModel pagingModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var item = GetCollection(distributionKey);
            var result = item.RequestResultModels.OrderByDescending(r => r.RequestBeginTime).ToArray();
            if (pagingModel != null)
            {
                IEnumerable<RequestResultModel> resultQuery = result;
                if (!string.IsNullOrWhiteSpace(pagingModel.FromIdentifier))
                {
                    var index = result
                        .Select((request, index) => new { request, index })
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

            return result.AsEnumerable().AsTask();
        }
    }

    /// <inheritdoc />
    public override Task<IEnumerable<(StubModel Stub, Dictionary<string, string> Metadata)>> GetStubsAsync(
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            // We need to convert the list to an array here, or else we can get errors when deleting the stubs.
            return GetCollection(distributionKey).StubModels.Select(s => (s, new Dictionary<string, string>()))
                .ToArray().AsEnumerable().AsTask();
        }
    }

    /// <inheritdoc />
    public override async Task<IEnumerable<(StubOverviewModel Stub, Dictionary<string, string> Metadata)>>
        GetStubsOverviewAsync(string distributionKey = null,
            CancellationToken cancellationToken = default) =>
        (await GetStubsAsync(distributionKey, cancellationToken))
        .Select(s => (new StubOverviewModel { Id = s.Stub.Id, Tenant = s.Stub.Tenant, Enabled = s.Stub.Enabled },
            s.Metadata))
        .ToArray();

    /// <inheritdoc />
    public override Task<(StubModel Stub, Dictionary<string, string> Metadata)?> GetStubAsync(string stubId,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var item = GetCollection(distributionKey);
        var stub = item.StubModels.FirstOrDefault(s => s.Id == stubId);
        (StubModel, Dictionary<string, string>)?
            result = stub != null ? (stub, new Dictionary<string, string>()) : null;
        return result.AsTask();
    }

    /// <inheritdoc />
    public override Task CleanOldRequestResultsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            foreach (var item in _collectionItems)
            {
                var maxLength = options.CurrentValue.Storage?.OldRequestsQueueLength ?? 40;
                var requests = item.Value.RequestResultModels
                    .OrderByDescending(r => r.RequestEndTime)
                    .Skip(maxLength);
                foreach (var request in requests)
                {
                    item.Value.RequestResultModels.Remove(request);
                    RemoveResponse(request, null);
                }
            }

            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public override Task<ScenarioStateModel> GetScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return ((ScenarioStateModel)null).AsTask();
        }

        var lookupKey = scenario.ToLower();
        var item = GetCollection(distributionKey);
        var result = !item.Scenarios.TryGetValue(lookupKey, out var value) ? null : value.Copy();
        return result.AsTask();
    }

    /// <inheritdoc />
    public override Task<ScenarioStateModel> AddScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var lookupKey = scenario.ToLower();
        var scenarioToAdd = scenarioStateModel.Copy();
        var item = GetCollection(distributionKey);
        if (!item.Scenarios.TryAdd(lookupKey, scenarioToAdd))
        {
            throw new InvalidOperationException($"Scenario state with key '{lookupKey}' already exists.");
        }

        return scenarioToAdd.AsTask();
    }

    /// <inheritdoc />
    public override Task UpdateScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var lookupKey = scenario.ToLower();
        var item = GetCollection(distributionKey);
        if (!item.Scenarios.TryGetValue(lookupKey, out var value))
        {
            return Task.CompletedTask;
        }

        var newScenarioState = scenarioStateModel.Copy();
        if (!item.Scenarios.TryUpdate(lookupKey, newScenarioState, value))
        {
            throw new InvalidOperationException(
                $"Something went wrong with updating scenario with key '{lookupKey}'.");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<ScenarioStateModel>> GetAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        GetCollection(distributionKey).Scenarios.Values.Select(i => i.Copy()).AsTask();

    /// <inheritdoc />
    public override Task<bool> DeleteScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default) =>
        string.IsNullOrWhiteSpace(scenario)
            ? false.AsTask()
            : GetCollection(distributionKey).Scenarios.TryRemove(scenario.ToLower(), out _).AsTask();

    /// <inheritdoc />
    public override Task DeleteAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var item = GetCollection(distributionKey);
        item.Scenarios.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task PrepareStubSourceAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void RemoveResponse(RequestResultModel request, string distributionKey)
    {
        var item = GetCollection(distributionKey);
        if (!item.RequestResponseMap.TryGetValue(request, out var value))
        {
            return;
        }

        item.StubResponses.Remove(value);
        item.RequestResponseMap.Remove(request);
    }

    private RequestResultModel GetRequest(string correlationId, string distributionKey)
    {
        var item = GetCollection(distributionKey);
        return item.RequestResultModels.FirstOrDefault(r => r.CorrelationId == correlationId);
    }

    internal StubRequestCollectionItem GetCollection(string distributionKey) =>
        _collectionItems.GetOrAdd(distributionKey ?? string.Empty,
            key => new StubRequestCollectionItem(key));
}

internal class StubRequestCollectionItem
{
    public readonly IDictionary<RequestResultModel, ResponseModel> RequestResponseMap =
        new Dictionary<RequestResultModel, ResponseModel>();

    public readonly IList<RequestResultModel> RequestResultModels = new List<RequestResultModel>();
    public readonly ConcurrentDictionary<string, ScenarioStateModel> Scenarios = new();
    public readonly IList<StubModel> StubModels = new List<StubModel>();
    public readonly IList<ResponseModel> StubResponses = new List<ResponseModel>();

    internal StubRequestCollectionItem(string key)
    {
        Key = key;
    }

    public string Key { get; set; }
}
