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

        public Task AddRequestResultAsync(RequestResultModel requestResult)
        {
            throw new NotImplementedException();
        }

        public Task AddStubAsync(StubModel stub)
        {
            throw new NotImplementedException();
        }

        public Task CleanOldRequestResultsAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllRequestResultsAsync()
        {
            throw new NotImplementedException();
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
