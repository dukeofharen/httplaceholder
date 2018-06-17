using System.Collections.Generic;
using Placeholder.Models;

namespace Placeholder.DataLogic
{
   public interface IStubContainer
   {
      IEnumerable<StubModel> GetStubs();

      string GetStubFileDirectory();
   }
}
