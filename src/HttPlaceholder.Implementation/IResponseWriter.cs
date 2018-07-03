using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.Implementation
{
   public interface IResponseWriter
   {
      Task WriteToResponseAsync(StubModel stub, ResponseModel response);
   }
}
