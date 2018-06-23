using System.Collections.Generic;
using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.DataLogic
{
   public interface IStubSource
   {
      Task<IEnumerable<StubModel>> GetStubsAsync();
   }
}
