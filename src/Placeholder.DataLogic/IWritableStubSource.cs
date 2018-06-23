using System.Collections.Generic;
using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.DataLogic
{
   public interface IWritableStubSource : IStubSource
   {
      Task AddStubAsync(StubModel stub);

      Task<bool> DeleteStubAsync(string stubId);

      Task AddRequestResultAsync(RequestResultModel requestResult);

      Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();
   }
}
