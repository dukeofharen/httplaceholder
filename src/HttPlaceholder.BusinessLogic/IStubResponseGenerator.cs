using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic
{
    public interface IStubResponseGenerator
    {
        Task<ResponseModel> GenerateResponseAsync(StubModel stub);
    }
}