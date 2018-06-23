using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.DataLogic.Implementations.StubSources
{
   internal class InMemoryStubSource : IWritableStubSource
   {
      private readonly IList<StubModel> _stubModels = new List<StubModel>();

      public Task AddStubAsync(StubModel stub)
      {
         _stubModels.Add(stub);
         return Task.CompletedTask;
      }

      public Task<IEnumerable<StubModel>> GetStubsAsync()
      {
         return Task.FromResult(_stubModels.AsEnumerable());
      }
   }
}
