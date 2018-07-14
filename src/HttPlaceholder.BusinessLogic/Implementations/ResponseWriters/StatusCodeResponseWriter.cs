using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   public class StatusCodeResponseWriter : IResponseWriter
   {
      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if (response.StatusCode == 0)
         {
            response.StatusCode = stub.Response?.StatusCode ?? 200;
         }

         return Task.CompletedTask;
      }
   }
}
