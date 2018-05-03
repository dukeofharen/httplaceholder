using System.Collections.Generic;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubContainer
   {
      IEnumerable<StubModel> Stubs { get; }

      StubModel GetStubById(string id);
   }
}
