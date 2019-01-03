using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.DataLogic.Implementations.StubSources
{
    internal class RelationalDbStubSource : IWritableStubSource
    {
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

        public Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StubModel>> GetStubsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
