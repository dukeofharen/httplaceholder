using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   public class TextResponseWriter : IResponseWriter
   {
      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if(stub.Response.Text != null)
         {
            response.Body = Encoding.UTF8.GetBytes(stub.Response.Text);
            if (!response.Headers.TryGetValue("Content-Type", out string contentType))
            {
               response.Headers.Add("Content-Type", "text/plain");
            }
         }

         return Task.CompletedTask;
      }
   }
}
