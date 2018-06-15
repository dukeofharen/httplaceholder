using System.Threading.Tasks;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations.ResponseWriters
{
    public class ExtraDurationResponseWriter : IResponseWriter
    {
      private readonly IAsyncService _asyncService;
      private readonly IRequestLoggerFactory _requestLoggerFactory;

      public ExtraDurationResponseWriter(
         IAsyncService asyncService,
         IRequestLoggerFactory requestLoggerFactory)
      {
         _asyncService = asyncService;
         _requestLoggerFactory = requestLoggerFactory;
      }

      public async Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         // Simulate sluggish response here, if configured.
         if (stub.Response?.ExtraDuration.HasValue == true)
         {

            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            int duration = stub.Response.ExtraDuration.Value;
            requestLogger.Log($"Waiting '{duration}' extra milliseconds.");
            await _asyncService.DelayAsync(duration);
         }
      }
   }
}
