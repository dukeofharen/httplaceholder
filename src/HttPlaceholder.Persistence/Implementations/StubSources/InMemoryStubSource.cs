using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Persistence.Implementations.StubSources;

/// <summary>
///     A stub source that is used to store and read data from memory.
/// </summary>
internal class InMemoryStubSource(IOptionsMonitor<SettingsModel> options) : BaseWritableStubSource
{
    private static readonly object _lock = new();
    private readonly ConcurrentDictionary<string, StubRequestCollectionItem> CollectionItems = new();

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
            return Task.FromResult(GetRequest(correlationId, distributionKey));
        }
    }

    /// <inheritdoc />
    public override Task<ResponseModel> GetResponseAsync(string correlationId, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var nullValue = Task.FromResult((ResponseModel)null);
            var request = GetRequest(correlationId, distributionKey);
            if (request == null)
            {
                return nullValue;
            }

            var item = GetCollection(distributionKey);
            return !item.RequestResponseMap.ContainsKey(request)
                ? nullValue
                : Task.FromResult(item.RequestResponseMap[request]);
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
                return Task.FromResult(false);
            }

            item.RequestResultModels.Remove(request);
            RemoveResponse(request, distributionKey);
            return Task.FromResult(true);
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
                return Task.FromResult(false);
            }

            item.StubModels.Remove(stub);
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

            return Task.FromResult(result.AsEnumerable());
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
            var item = GetCollection(distributionKey);
            var stubs = item.StubModels;
            return Task.FromResult(stubs.Select(s => (s, new Dictionary<string, string>())).ToArray().AsEnumerable());
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
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public override Task CleanOldRequestResultsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            foreach (var item in CollectionItems)
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
            return Task.FromResult((ScenarioStateModel)null);
        }

        var lookupKey = scenario.ToLower();
        var item = GetCollection(distributionKey);
        var result = !item.Scenarios.ContainsKey(lookupKey) ? null : item.Scenarios[lookupKey].Copy();
        return Task.FromResult(result);
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

        return Task.FromResult(scenarioToAdd);
    }

    /// <inheritdoc />
    public override Task UpdateScenarioAsync(string scenario, ScenarioStateModel scenarioStateModel,
        string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var lookupKey = scenario.ToLower();
        var item = GetCollection(distributionKey);
        if (!item.Scenarios.ContainsKey(lookupKey))
        {
            return Task.CompletedTask;
        }

        var existingScenarioState = item.Scenarios[lookupKey];
        var newScenarioState = scenarioStateModel.Copy();
        if (!item.Scenarios.TryUpdate(lookupKey, newScenarioState, existingScenarioState))
        {
            throw new InvalidOperationException(
                $"Something went wrong with updating scenario with key '{lookupKey}'.");
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override Task<IEnumerable<ScenarioStateModel>> GetAllScenariosAsync(string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        var item = GetCollection(distributionKey);
        return Task.FromResult(item.Scenarios.Values.Select(i => i.Copy()));
    }

    /// <inheritdoc />
    public override Task<bool> DeleteScenarioAsync(string scenario, string distributionKey = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scenario))
        {
            return Task.FromResult(false);
        }

        var lookupKey = scenario.ToLower();
        var item = GetCollection(distributionKey);
        return Task.FromResult(item.Scenarios.TryRemove(lookupKey, out _));
    }

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
        if (!item.RequestResponseMap.ContainsKey(request))
        {
            return;
        }

        var response = item.RequestResponseMap[request];
        item.StubResponses.Remove(response);
        item.RequestResponseMap.Remove(request);
    }

    private RequestResultModel GetRequest(string correlationId, string distributionKey)
    {
        var item = GetCollection(distributionKey);
        return item.RequestResultModels.FirstOrDefault(r => r.CorrelationId == correlationId);
    }

    internal StubRequestCollectionItem GetCollection(string distributionKey) =>
        CollectionItems.GetOrAdd(distributionKey ?? string.Empty,
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
