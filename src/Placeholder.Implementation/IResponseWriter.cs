using System.Threading.Tasks;
using Placeholder.Models;

namespace Placeholder.Implementation
{
   public interface IResponseWriter
   {
      Task WriteToResponseAsync(StubModel stub, ResponseModel response);
   }
}
