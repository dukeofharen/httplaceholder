using System.Collections.Generic;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubContainer
   {
      IEnumerable<StubModel> GetStubs();

      string GetStubFileDirectory();
   }
}
