using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Db.Implementations;

/// <inheritdoc />
internal class RelationalDbStubCache(IQueryStore queryStore, ILogger<RelationalDbStubCache> logger)
    : IRelationalDbStubCache
{
    private static readonly object _cacheUpdateLock = new();

    internal readonly ConcurrentDictionary<string, StubModel> StubCache = new();
    internal string StubUpdateTrackingId;

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
            logger.LogWarning($"Could not add stub with ID '{stubModel.Id}' to cache.");
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
    public async Task<IEnumerable<StubModel>> GetOrUpdateStubCacheAsync(string distributionKey, IDatabaseContext ctx,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(distributionKey))
        {
            return GetStubs(ctx, distributionKey);
        }

        var shouldUpdateCache = false;

        // Check if the "stub_update_tracking_id" field in the database has a new value.
        var stubUpdateTrackingId =
            await ctx.QueryFirstOrDefaultAsync<string>(queryStore.GetStubUpdateTrackingIdQuery, cancellationToken);
        if (string.IsNullOrWhiteSpace(stubUpdateTrackingId))
        {
            lock (_cacheUpdateLock)
            {
                // ID doesn't exist yet. Create one and persist it.
                logger.LogDebug("Initializing the cache, because there is no tracking ID in the database yet.");
                var newId = Guid.NewGuid().ToString();
                StubUpdateTrackingId = newId;
                ctx.Execute(
                    queryStore.InsertStubUpdateTrackingIdQuery,
                    new { StubUpdateTrackingId = newId });
                shouldUpdateCache = true;
            }
        }
        else if (StubUpdateTrackingId == null)
        {
            // The local cache hasn't been initialized yet. Do that now.
            logger.LogDebug(
                "Initializing the cache, because either the local stub cache or tracking ID is not set yet.");
            StubUpdateTrackingId = stubUpdateTrackingId;
            shouldUpdateCache = true;
        }
        else if (StubUpdateTrackingId != stubUpdateTrackingId)
        {
            lock (_cacheUpdateLock)
            {
                // ID has been changed. Update the stub cache.
                logger.LogDebug(
                    "Initializing the cache, because the tracking ID in the database has been changed.");
                StubUpdateTrackingId = stubUpdateTrackingId;
                shouldUpdateCache = true;
            }
        }

        if (!shouldUpdateCache)
        {
            return StubCache.Values;
        }

        StubCache.Clear();
        var stubs = GetStubs(ctx, string.Empty);
        foreach (var stub in stubs)
        {
            if (!StubCache.TryAdd(stub.Id, stub))
            {
                logger.LogWarning($"Could not add stub with ID '{stub.Id}' to cache.");
            }
        }

        return StubCache.Values;
    }

    private IList<StubModel> GetStubs(IDatabaseContext ctx, string distributionKey)
    {
        var queryResults = ctx.Query<DbStubModel>(queryStore.GetStubsQuery, new { DistributionKey = distributionKey });

        return queryResults.Select(queryResult => queryResult.StubType switch
            {
                StubTypes.StubJsonType => JsonConvert.DeserializeObject<StubModel>(queryResult.Stub),
                StubTypes.StubYamlType => YamlUtilities.Parse<StubModel>(queryResult.Stub),
                _ => throw new NotImplementedException(
                    $"StubType '{queryResult.StubType}' not supported: stub '{queryResult.StubId}'.")
            })
            .ToList();
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
        await ctx.ExecuteAsync(queryStore.UpdateStubUpdateTrackingIdQuery, cancellationToken,
            new { StubUpdateTrackingId = newId });
        return newId;
    }
}
