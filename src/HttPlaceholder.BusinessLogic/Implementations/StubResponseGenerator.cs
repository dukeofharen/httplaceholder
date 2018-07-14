using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations
{
   internal class StubResponseGenerator : IStubResponseGenerator
   {
      private readonly IServiceProvider _serviceProvider;

      public StubResponseGenerator(IServiceProvider serviceProvider)
      {
         _serviceProvider = serviceProvider;
      }

      public async Task<ResponseModel> GenerateResponseAsync(StubModel stub)
      {
         var response = new ResponseModel();
         var conditionCheckers = ((IEnumerable<IResponseWriter>)_serviceProvider.GetServices(typeof(IResponseWriter))).ToArray();
         foreach(var checker in conditionCheckers)
         {
            await checker.WriteToResponseAsync(stub, response);
         }

         return response;
      }
   }
}
