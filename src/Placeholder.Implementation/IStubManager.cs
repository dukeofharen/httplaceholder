using System.Collections.Generic;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubManager
   {
      IEnumerable<StubModel> Stubs { get; }

      StubModel GetStubById(string id);

      IEnumerable<StubModel> GetStubsByIds(IEnumerable<string> ids);
   }
}
