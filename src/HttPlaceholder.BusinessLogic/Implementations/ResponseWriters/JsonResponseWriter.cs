using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Models;
using HttPlaceholder.Services;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
   internal class JsonResponseWriter : IResponseWriter
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;

      public JsonResponseWriter(IRequestLoggerFactory requestLoggerFactory)
      {
         _requestLoggerFactory = requestLoggerFactory;
      }

      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         if(stub.Response?.Json != null)
         {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            string jsonBody = stub.Response.Json;
            response.Body = Encoding.UTF8.GetBytes(jsonBody);
            string bodyForLogging = jsonBody.Length > 10 ? jsonBody.Substring(0, 10) : jsonBody;
            requestLogger.Log($"Found JSON body: {bodyForLogging}");
            if(!response.Headers.TryGetValue("Content-Type", out string contentType))
            {
               response.Headers.Add("Content-Type", "application/json");
            }
         }

         return Task.CompletedTask;
      }
   }
}
