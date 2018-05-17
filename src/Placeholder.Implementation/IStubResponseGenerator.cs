using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubResponseGenerator
   {
      Task<ResponseModel> GenerateResponseAsync(StubModel stub);
   }
}
