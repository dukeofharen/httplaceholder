using System.Collections.Generic;
using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubContainer
   {
      Task<IEnumerable<StubModel>> GetStubsAsync();
   }
}
