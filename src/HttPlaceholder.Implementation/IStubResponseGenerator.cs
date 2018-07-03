using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation
{
   public interface IStubResponseGenerator
   {
      Task<ResponseModel> GenerateResponseAsync(StubModel stub);
   }
}
