using System.Threading.Tasks;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters
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

        public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            // Simulate sluggish response here, if configured.
            if (stub.Response?.ExtraDuration.HasValue != true)
            {
                return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
            }

            var duration = stub.Response.ExtraDuration.Value;
            await _asyncService.DelayAsync(duration);
            return StubResponseWriterResultModel.IsExecuted(GetType().Name);
        }
    }
}
