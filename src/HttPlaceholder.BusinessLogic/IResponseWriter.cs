using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic
{
   public interface IResponseWriter
   {
      Task WriteToResponseAsync(StubModel stub, ResponseModel response);
   }
}
