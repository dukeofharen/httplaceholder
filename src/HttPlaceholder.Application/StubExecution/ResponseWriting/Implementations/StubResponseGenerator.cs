using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriting.Implementations
{
    internal class StubResponseGenerator : IStubResponseGenerator
    {
        private readonly IRequestLoggerFactory _requestLoggerFactory;
        private readonly IEnumerable<IResponseWriter> _responseWriters;

        public StubResponseGenerator(
            IRequestLoggerFactory requestLoggerFactory,
            IEnumerable<IResponseWriter> responseWriters)
        {
            _requestLoggerFactory = requestLoggerFactory;
            _responseWriters = responseWriters;
        }

        public async Task<ResponseModel> GenerateResponseAsync(StubModel stub)
        {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            var response = new ResponseModel();
            foreach (var writer in _responseWriters.OrderByDescending(w => w.Priority))
            {
                var executed = await writer.WriteToResponseAsync(stub, response);
                requestLogger.SetResponseWriterResult(writer.GetType().Name, executed);
            }

            return response;
        }
    }
}
