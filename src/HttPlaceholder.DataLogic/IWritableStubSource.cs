using System.Collections.Generic;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.DataLogic
{
   public interface IWritableStubSource : IStubSource
   {
      Task AddStubAsync(StubModel stub);

      Task<bool> DeleteStubAsync(string stubId);

      Task AddRequestResultAsync(RequestResultModel requestResult);

      Task<IEnumerable<RequestResultModel>> GetRequestResultsAsync();

      Task DeleteAllRequestResultsAsync();

      Task CleanOldRequestResultsAsync();
   }
}
