using System;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    public class LineEndingResponseWriter : IResponseWriter
    {
        private readonly ILogger<LineEndingResponseWriter> _logger;

        public LineEndingResponseWriter(ILogger<LineEndingResponseWriter> logger)
        {
            _logger = logger;
        }

        public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
        {
            throw new NotImplementedException();
            // if (string.IsNullOrWhiteSpace(stub.Response.LineEndings))
            // {
            //     return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
            // }
            //
            // return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
        }

        public int Priority => -10;
    }
}
