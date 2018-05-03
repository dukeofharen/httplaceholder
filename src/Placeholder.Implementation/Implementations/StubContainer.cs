using System.Collections.Generic;
using System.Linq;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   public class StubContainer : IStubContainer
   {
      public StubContainer(IEnumerable<StubModel> stubs)
      {
         Stubs = stubs;
      }

      public IEnumerable<StubModel> Stubs { get; }

      public StubModel GetStubById(string id)
      {
         return Stubs.FirstOrDefault(s => s.Id == id);
      }

      public IEnumerable<StubModel> GetStubsByIds(IEnumerable<string> ids)
      {
         return Stubs.Where(s => ids.Contains(s.Id));
      }
   }
}
