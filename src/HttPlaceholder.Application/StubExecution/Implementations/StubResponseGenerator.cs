using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriting;
using HttPlaceholder.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.Application.StubExecution.Implementations
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
                bool executed = await writer.WriteToResponseAsync(stub, response);
                requestLogger.SetResponseWriterResult(writer.GetType().Name, executed);
            }

            return response;
        }
    }
}
