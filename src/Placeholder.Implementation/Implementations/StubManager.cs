using System.Collections.Generic;
using System.Linq;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   public class StubManager : IStubManager
   {
      private readonly IStubContainer _stubContainer;

      public StubManager(IStubContainer stubContainer)
      {
         _stubContainer = stubContainer;
      }

      public IEnumerable<StubModel> Stubs => _stubContainer.GetStubs();

      public StubModel GetStubById(string id)
      {
         return Stubs.FirstOrDefault(s => s.Id == id);
      }

      public IEnumerable<StubModel> GetStubsByIds(IEnumerable<string> ids)
      {
         return ids == null ? Stubs : Stubs.Where(s => ids.Contains(s.Id));
      }
   }
}
