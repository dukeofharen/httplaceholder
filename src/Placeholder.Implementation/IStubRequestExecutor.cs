using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubRequestExecutor
   {
      Task<ResponseModel> ExecuteRequestAsync();
   }
}
