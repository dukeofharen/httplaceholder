using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   public class HeadersResponseWriter : IResponseWriter
   {
      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         var stubResponseHeaders = stub.Response?.Headers;
         if (stubResponseHeaders != null)
         {
            foreach (var header in stubResponseHeaders)
            {
               response.Headers.Add(header.Key, header.Value);
            }
         }

         return Task.CompletedTask;
      }
   }
}
