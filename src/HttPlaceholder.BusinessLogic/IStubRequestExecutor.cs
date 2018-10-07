using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic
{
    public interface IStubRequestExecutor
    {
        Task<ResponseModel> ExecuteRequestAsync();
    }
}