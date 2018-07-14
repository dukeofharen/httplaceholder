using System.Threading.Tasks;
using HttPlaceholder.Services;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations.ResponseWriters
{
    public class ExtraDurationResponseWriter : IResponseWriter
    {
      private readonly IAsyncService _asyncService;

      public ExtraDurationResponseWriter(
         IAsyncService asyncService)
      {
         _asyncService = asyncService;
      }

      public async Task WriteToResponseAsync(StubModel stub, ResponseModel response)
      {
         // Simulate sluggish response here, if configured.
         if (stub.Response?.ExtraDuration.HasValue == true)
         {
            int duration = stub.Response.ExtraDuration.Value;
            await _asyncService.DelayAsync(duration);
         }
      }
   }
}
