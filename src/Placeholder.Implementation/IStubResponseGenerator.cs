using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IStubResponseGenerator
   {
      ResponseModel GenerateResponse(StubModel stub);
   }
}
