using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
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
         requestLogger.Log($"Stub with ID '{stub.Id}' found; returning response.");

         var response = new ResponseModel();
         var conditionCheckers = ((IEnumerable<IResponseWriter>)_serviceProvider.GetServices(typeof(IResponseWriter))).ToArray();
         foreach(var checker in conditionCheckers)
         {
            requestLogger.Log($"Executing response writer '{checker.GetType()}'");
            await checker.WriteToResponseAsync(stub, response);
         }

         return response;
      }
   }
}
