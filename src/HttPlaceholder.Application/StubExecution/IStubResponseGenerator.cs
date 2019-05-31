using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution
{
    public interface IStubResponseGenerator
    {
        Task<ResponseModel> GenerateResponseAsync(StubModel stub);
    }
}
