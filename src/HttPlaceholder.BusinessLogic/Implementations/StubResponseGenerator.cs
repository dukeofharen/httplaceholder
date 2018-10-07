using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Models;
using HttPlaceholder.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HttPlaceholder.BusinessLogic.Implementations
{
    internal class StubResponseGenerator : IStubResponseGenerator
    {
        private readonly IRequestLoggerFactory _requestLoggerFactory;
        private readonly IServiceProvider _serviceProvider;

        public StubResponseGenerator(
           IRequestLoggerFactory requestLoggerFactory,
           IServiceProvider serviceProvider)
        {
            _requestLoggerFactory = requestLoggerFactory;
            _serviceProvider = serviceProvider;
        }

        public async Task<ResponseModel> GenerateResponseAsync(StubModel stub)
        {
            var requestLogger = _requestLoggerFactory.GetRequestLogger();
            var response = new ResponseModel();
            var responseWriters = ((IEnumerable<IResponseWriter>)_serviceProvider.GetServices(typeof(IResponseWriter))).ToArray();
            foreach (var writer in responseWriters)
            {
                bool executed = await writer.WriteToResponseAsync(stub, response);
                requestLogger.SetResponseWriterResult(writer.GetType().Name, executed);
            }

            return response;
        }
    }
}