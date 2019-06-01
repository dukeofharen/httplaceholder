using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Ducode.Essentials.Console;
using HttPlaceholder.Application.Interfaces;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Persistence.Db;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Persistence.Implementations.StubSources
{
    internal class RelationalDbStubSource : IWritableStubSource
    {
        private const string StubJsonType = "json";
        private const string StubYamlType = "yaml";

        private readonly SettingsModel _settings;
        private readonly IQueryStore _queryStore;

        public RelationalDbStubSource(
            IOptions<SettingsModel> options,
            IQueryStore queryStore)
        {
            _settings = options.Value;
            _queryStore = queryStore;
        }

        public async Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            using (var conn = _queryStore.GetConnection())
            {
                string json = JsonConvert.SerializeObject(requestResult);
                await conn.ExecuteAsync(_queryStore.AddRequestQuery, new
                {
                    requestResult.CorrelationId,
                    requestResult.ExecutingStubId,
                    requestResult.RequestBeginTime,
                    requestResult.RequestEndTime,
                    Json = json
                });
            }
        }

        public async Task AddStubAsync(StubModel stub)
        {
            using (var conn = _queryStore.GetConnection())
            {
                string json = JsonConvert.SerializeObject(stub);
                await conn.ExecuteAsync(_queryStore.AddStubQuery, new
                {
                    StubId = stub.Id,
                    Stub = json,
                    StubType = StubJsonType
                });
            }
        }

        public async Task CleanOldRequestResultsAsync()
        {
            int maxLength = _settings.Storage?.OldRequestsQueueLength ?? 40;
            using (var conn = _queryStore.GetConnection())
            {
                await conn.ExecuteAsync(_queryStore.CleanOldRequestsQuery, new { Limit = maxLength });
            }
        }

        public async Task DeleteAllRequestResultsAsync()
        {
            using (var conn = _queryStore.GetConnection())
            {
                await conn.ExecuteAsync(_queryStore.DeleteAllRequestsQuery);
            }
        }

        public async Task<bool> DeleteStubAsync(string stubId)
        {
            using (var conn = _queryStore.GetConnection())
            {
                int updated = await conn.ExecuteAsync(_queryStore.DeleteStubQuery, new { StubId = stubId });
                return updated > 0;
            }
        }

        public async Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
        {
            using (var conn = _queryStore.GetConnection())
            {
                var result = await conn.QueryAsync<DbRequestModel>(_queryStore.GetRequestsQuery);
                return result
                    .Select(r => JsonConvert.DeserializeObject<RequestResultModel>(r.Json));
            }
        }

        public async Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            using (var conn = _queryStore.GetConnection())
            {
                var queryResults = await conn.QueryAsync<DbStubModel>(_queryStore.GetStubsQuery);
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
                            throw new NotImplementedException($"StubType '{queryResult.StubType}' not supported: stub '{queryResult.StubId}'.");
                    }
                }

                return result;
            }
        }

        public async Task PrepareStubSourceAsync()
        {
            using (var conn = _queryStore.GetConnection())
            {
                await conn.ExecuteAsync(_queryStore.MigrationsQuery);
            }
        }
    }
}
