using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    public interface IStubRequestExecutor
    {
        Task<ResponseModel> ExecuteRequestAsync();
    }
}
