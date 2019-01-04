using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HttPlaceholder.DataLogic.Db;
using HttPlaceholder.DataLogic.Db.Models;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Newtonsoft.Json;

namespace HttPlaceholder.DataLogic.Implementations.StubSources
{
    internal class RelationalDbStubSource : IWritableStubSource
    {
        private const string StubJsonType = "json";
        private const string StubYamlType = "yaml";

        private readonly IQueryStore _queryStore;
        private readonly IYamlService _yamlService;

        public RelationalDbStubSource(
            IQueryStore queryStore,
            IYamlService yamlService)
        {
            _queryStore = queryStore;
            _yamlService = yamlService;
        }

        public async Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            using (var conn = _queryStore.GetConnection())
            {
                string json = JsonConvert.SerializeObject(requestResult);
                await conn.ExecuteAsync(_queryStore.AddRequestQuery, new
                {
                    CorrelationId = requestResult.CorrelationId,
                    ExecutingStubId = requestResult.ExecutingStubId,
                    RequestBeginTime = requestResult.RequestBeginTime,
                    RequestEndTime = requestResult.RequestEndTime,
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

        public Task CleanOldRequestResultsAsync()
        {
            return Task.CompletedTask;
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
                int updated = await conn.ExecuteAsync(_queryStore.DeletStubQuery, new { StubId = stubId });
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
                            result.Add(_yamlService.Parse<StubModel>(queryResult.Stub));
                            break;
                        default:
                            throw new NotImplementedException($"StubType '{queryResult.StubType}' not supported: stub '{queryResult.StubId}'.");
                    }
                }

                return result;
            }
        }
    }
}
