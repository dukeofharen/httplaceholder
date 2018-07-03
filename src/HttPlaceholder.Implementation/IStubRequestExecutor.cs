using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation
{
   public interface IStubRequestExecutor
   {
      Task<ResponseModel> ExecuteRequestAsync();
   }
}
