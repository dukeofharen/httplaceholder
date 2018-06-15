using System.Threading.Tasks;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations.ResponseWriters
{
    public class HeadersResponseWriter : IResponseWriter
    {
      private readonly IRequestLoggerFactory _requestLoggerFactory;

      public HeadersResponseWriter(IRequestLoggerFactory requestLoggerFactory)
      {
         _requestLoggerFactory = requestLoggerFactory;
      }

      public Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var stubResponseHeaders = stub.Response?.Headers;
         if (stubResponseHeaders != null)
         {
            foreach (var header in stubResponseHeaders)
            {
               requestLogger.Log($"Found header '{header.Key}' with value '{header.Value}'.");
               response.Headers.Add(header.Key, header.Value);
            }
         }

         return Task.CompletedTask;
      }
   }
}
