using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <inheritdoc />
internal class RelationalDbStubCache : IRelationalDbStubCache
{
    private const string StubJsonType = "json";
    private const string StubYamlType = "yaml";

    private static readonly object _cacheUpdateLock = new();
    private readonly ILogger<RelationalDbStubCache> _logger;

    private readonly IQueryStore _queryStore;
    internal readonly ConcurrentDictionary<string, StubModel> StubCache = new();
    internal string StubUpdateTrackingId;

    public RelationalDbStubCache(IQueryStore queryStore, ILogger<RelationalDbStubCache> logger)
    {
        _queryStore = queryStore;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task AddOrReplaceStubAsync(IDatabaseContext ctx, StubModel stubModel,
        CancellationToken cancellationToken)
    {
        var item = StubCache.ContainsKey(stubModel.Id) ? StubCache[stubModel.Id] : null;
        if (item != null)
        {
            StubCache.Remove(stubModel.Id, out _);
        }

        if (!StubCache.TryAdd(stubModel.Id, stubModel))
        {
            _logger.LogWarning($"Could not add stub with ID '{stubModel.Id}' to cache.");
        }

        var newId = await UpdateTrackingIdAsync(ctx, cancellationToken);
        UpdateLocalStubUpdateTrackingId(newId);
    }

    /// <inheritdoc />
    public async Task DeleteStubAsync(IDatabaseContext ctx, string stubId, CancellationToken cancellationToken)
    {
        if (StubCache.TryRemove(stubId, out _))
        {
            var newId = await UpdateTrackingIdAsync(ctx, cancellationToken);
            UpdateLocalStubUpdateTrackingId(newId);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<StubModel>> GetOrUpdateStubCacheAsync(IDatabaseContext ctx,
        CancellationToken cancellationToken)
    {
        var shouldUpdateCache = false;

        // Check if the "stub_update_tracking_id" field in the database has a new value.
        var stubUpdateTrackingId =
            await ctx.QueryFirstOrDefaultAsync<string>(_queryStore.GetStubUpdateTrackingIdQuery, cancellationToken);
        if (string.IsNullOrWhiteSpace(stubUpdateTrackingId))
        {
            lock (_cacheUpdateLock)
            {
                // ID doesn't exist yet. Create one and persist it.
                _logger.LogInformation("Initializing the cache, because there is no tracking ID in the database yet.");
                var newId = Guid.NewGuid().ToString();
                StubUpdateTrackingId = newId;
                ctx.Execute(
                    _queryStore.InsertStubUpdateTrackingIdQuery,
                    new {StubUpdateTrackingId = newId});
                shouldUpdateCache = true;
            }
        }
        else if (StubUpdateTrackingId == null)
        {
            // The local cache hasn't been initialized yet. Do that now.
            _logger.LogInformation(
                "Initializing the cache, because either the local stub cache or tracking ID is not set yet.");
            StubUpdateTrackingId = stubUpdateTrackingId;
            shouldUpdateCache = true;
        }
        else if (StubUpdateTrackingId != stubUpdateTrackingId)
        {
            lock (_cacheUpdateLock)
            {
                // ID has been changed. Update the stub cache.
                _logger.LogInformation(
                    "Initializing the cache, because the tracking ID in the database has been changed.");
                StubUpdateTrackingId = stubUpdateTrackingId;
                shouldUpdateCache = true;
            }
        }

        if (!shouldUpdateCache)
        {
            return StubCache.Values;
        }

        var queryResults = ctx.Query<DbStubModel>(_queryStore.GetStubsQuery);
        StubCache.Clear();
        foreach (var queryResult in queryResults)
        {
            var stub = queryResult.StubType switch
            {
                StubJsonType => JsonConvert.DeserializeObject<StubModel>(queryResult.Stub),
                StubYamlType => YamlUtilities.Parse<StubModel>(queryResult.Stub),
                _ => throw new NotImplementedException(
                    $"StubType '{queryResult.StubType}' not supported: stub '{queryResult.StubId}'.")
            };

            if (!StubCache.TryAdd(stub.Id, stub))
            {
                _logger.LogWarning($"Could not add stub with ID '{stub.Id}' to cache.");
            }
        }

        return StubCache.Values;
    }

    private void UpdateLocalStubUpdateTrackingId(string trackingId)
    {
        lock (_cacheUpdateLock)
        {
            StubUpdateTrackingId = trackingId;
        }
    }

    private async Task<string> UpdateTrackingIdAsync(IDatabaseContext ctx, CancellationToken cancellationToken)
    {
        var newId = Guid.NewGuid().ToString();
        await ctx.ExecuteAsync(_queryStore.UpdateStubUpdateTrackingIdQuery, cancellationToken,
            new {StubUpdateTrackingId = newId});
        return newId;
    }
}
