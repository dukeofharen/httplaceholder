using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   internal class JsonResponseWriter : IResponseWriter
   {
      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if (stub.Response?.Json != null)
         {
            string jsonBody = stub.Response.Json;
            response.Body = Encoding.UTF8.GetBytes(jsonBody);
            string bodyForLogging = jsonBody.Length > 10 ? jsonBody.Substring(0, 10) : jsonBody;
            if (!response.Headers.TryGetValue("Content-Type", out string contentType))
            {
               response.Headers.Add("Content-Type", "application/json");
            }
         }

         return Task.CompletedTask;
      }
   }
}
