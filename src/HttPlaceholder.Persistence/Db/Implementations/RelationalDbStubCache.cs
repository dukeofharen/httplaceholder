using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Db.Implementations
{
    internal class RelationalDbStubCache : IRelationalDbStubCache
    {
        private const string StubJsonType = "json";
        private const string StubYamlType = "yaml";

        private static readonly object _cacheUpdateLock = new object();
        private static string _stubUpdateTrackingId;
        private static IList<StubModel> _stubCache;

        private readonly IQueryStore _queryStore;

        public RelationalDbStubCache(IQueryStore queryStore)
        {
            _queryStore = queryStore;
        }

        public void ClearStubCache(IDatabaseContext ctx)
        {
            // Clear the in memory stub cache.
            lock (_cacheUpdateLock)
            {
                _stubCache = null;
                var newId = Guid.NewGuid().ToString();
                _stubUpdateTrackingId = newId;
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
                    var newId = Guid.NewGuid().ToString();
                    _stubUpdateTrackingId = newId;
                    ctx.Execute(
                        _queryStore.InsertStubUpdateTrackingIdQuery,
                        new {StubUpdateTrackingId = newId});
                    shouldUpdateCache = true;
                }
            }
            else if (_stubCache == null || _stubUpdateTrackingId == null)
            {
                // The local cache hasn't been initialized yet. Do that now.
                _stubUpdateTrackingId = stubUpdateTrackingId;
                shouldUpdateCache = true;
            }
            else if (_stubUpdateTrackingId != stubUpdateTrackingId)
            {
                // ID has been changed. Update the stub cache.
                lock (_cacheUpdateLock)
                {
                    _stubUpdateTrackingId = stubUpdateTrackingId;
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

                    _stubCache = result;
                }
            }

            return _stubCache;
        }
    }
}
