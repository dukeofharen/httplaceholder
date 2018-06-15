using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Implementation.Services;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   internal class StubResponseGenerator : IStubResponseGenerator
   {
      private readonly IAsyncService _asyncService;
      private readonly IFileService _fileService;
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubContainer _stubContainer;

      public StubResponseGenerator(
         IAsyncService asyncService,
         IFileService fileService,
         IRequestLoggerFactory requestLoggerFactory,
         IServiceProvider serviceProvider,
         IStubContainer stubContainer)
      {
         _asyncService = asyncService;
         _fileService = fileService;
         _requestLoggerFactory = requestLoggerFactory;
         _serviceProvider = serviceProvider;
         _stubContainer = stubContainer;
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
