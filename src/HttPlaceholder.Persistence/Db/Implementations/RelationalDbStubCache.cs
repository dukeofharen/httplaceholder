using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Db.Implementations;

internal class RelationalDbStubCache : IRelationalDbStubCache
{
    private const string StubJsonType = "json";
    private const string StubYamlType = "yaml";

    private static readonly object _cacheUpdateLock = new object();
    internal string StubUpdateTrackingId;
    internal IList<StubModel> StubCache;

    private readonly IQueryStore _queryStore;
    private readonly ILogger<RelationalDbStubCache> _logger;

    public RelationalDbStubCache(IQueryStore queryStore, ILogger<RelationalDbStubCache> logger)
    {
        _queryStore = queryStore;
        _logger = logger;
    }

    public void ClearStubCache(IDatabaseContext ctx)
    {
        // Clear the in memory stub cache.
        lock (_cacheUpdateLock)
        {
            _logger.LogInformation("Clearing the relational DB stub cache.");
            StubCache = null;
            var newId = Guid.NewGuid().ToString();
            StubUpdateTrackingId = newId;
            ctx.Execute(
                _queryStore.UpdateStubUpdateTrackingIdQuery,
                new {StubUpdateTrackingId = newId});
        }
    }

    public async Task<IEnumerable<StubModel>> GetOrUpdateStubCache(IDatabaseContext ctx)
    {
        var shouldUpdateCache = false;

        // Check if the "stub_update_tracking_id" field in the database has a new value.
        var stubUpdateTrackingId =
            await ctx.QueryFirstOrDefaultAsync<string>(_queryStore.GetStubUpdateTrackingIdQuery);
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
        else if (StubCache == null || StubUpdateTrackingId == null)
        {
            // The local cache hasn't been initialized yet. Do that now.
            _logger.LogInformation("Initializing the cache, because either the local stub cache or tracking ID is not set yet.");
            StubUpdateTrackingId = stubUpdateTrackingId;
            shouldUpdateCache = true;
        }
        else if (StubUpdateTrackingId != stubUpdateTrackingId)
        {
            lock (_cacheUpdateLock)
            {
                // ID has been changed. Update the stub cache.
                _logger.LogInformation("Initializing the cache, because the tracking ID in the database has been changed.");
                StubUpdateTrackingId = stubUpdateTrackingId;
                shouldUpdateCache = true;
            }
        }

        if (shouldUpdateCache)
        {
            lock (_cacheUpdateLock)
            {
                var queryResults = ctx.Query<DbStubModel>(_queryStore.GetStubsQuery);
                var result = new List<StubModel>();
                foreach (var queryResult in queryResults)
                {
                    switch (queryResult.StubType)
                    {
                        case StubJsonType:
                            result.Add(JsonConvert.DeserializeObject<StubModel>(queryResult.Stub));
                            break;

                        case StubYamlType:
                            result.Add(YamlUtilities.Parse<StubModel>(queryResult.Stub));
                            break;

                        default:
                            throw new NotImplementedException(
                                $"StubType '{queryResult.StubType}' not supported: stub '{queryResult.StubId}'.");
                    }
                }

                StubCache = result;
            }
        }

        return StubCache;
    }
}