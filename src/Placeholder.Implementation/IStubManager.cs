using System.Collections.Generic;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubManager
   {
      IEnumerable<StubModel> Stubs { get; }

      IEnumerable<StubModel> GetStubsByIds(IEnumerable<string> ids);
   }
}
