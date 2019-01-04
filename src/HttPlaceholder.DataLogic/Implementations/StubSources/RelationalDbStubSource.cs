using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using HttPlaceholder.DataLogic.Db;
using HttPlaceholder.DataLogic.Db.Models;
using HttPlaceholder.Models;
using Newtonsoft.Json;

namespace HttPlaceholder.DataLogic.Implementations.StubSources
{
    internal class RelationalDbStubSource : IWritableStubSource
    {
        private readonly IQueryStore _queryStore;

        public RelationalDbStubSource(IQueryStore queryStore)
        {
            _queryStore = queryStore;
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

        public Task AddStubAsync(StubModel stub)
        {
            throw new NotImplementedException();
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

        public Task<bool> DeleteStubAsync(string stubId)
        {
            throw new NotImplementedException();
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

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            return Task.FromResult(new StubModel[0].AsEnumerable());
        }
    }
}
