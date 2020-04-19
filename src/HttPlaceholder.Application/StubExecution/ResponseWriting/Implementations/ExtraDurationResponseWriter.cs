using System.Threading.Tasks;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class ExtraDurationResponseWriter : IResponseWriter
    {
        private readonly IAsyncService _asyncService;

        public ExtraDurationResponseWriter(
           IAsyncService asyncService)
        {
            _asyncService = asyncService;
        }

        public int Priority => 0;

        public async Task<bool> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            var executed = false;

            // Simulate sluggish response here, if configured.
            if (stub.Response?.ExtraDuration.HasValue == true)
            {
                var duration = stub.Response.ExtraDuration.Value;
                await _asyncService.DelayAsync(duration);
                executed = true;
            }

            return executed;
        }
    }
}
