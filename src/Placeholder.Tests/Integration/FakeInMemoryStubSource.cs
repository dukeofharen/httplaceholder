using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Placeholder.DataLogic;
using Placeholder.Models;

namespace Placeholder.Tests.Integration
{
   public class FakeInMemoryStubSource : IWritableStubSource
   {
      public IList<RequestResultModel> RequestResultModels { get; } = new List<RequestResultModel>();

      public IList<StubModel> StubModels = new List<StubModel>();

      public FakeInMemoryStubSource()
      {
      }

      public Task AddRequestResultAsync(RequestResultModel requestResult)
      {
         RequestResultModels.Add(requestResult);
         return Task.CompletedTask;
      }

      public Task AddStubAsync(StubModel stub)
      {
         StubModels.Add(stub);
         return Task.CompletedTask;
      }

      public Task<bool> DeleteStubAsync(string stubId)
      {
         var stub = StubModels.FirstOrDefault(s => s.Id == stubId);
         if (stub == null)
         {
            return Task.FromResult(false);
         }

         StubModels.Remove(stub);
         return Task.FromResult(true);
      }

      public Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync()
      {
         return Task.FromResult(RequestResultModels.AsEnumerable());
      }

      public Task<IEnumerable<StubModel>> GetStubsAsync()
      {
         return Task.FromResult(StubModels.AsEnumerable());
      }
   }
}
