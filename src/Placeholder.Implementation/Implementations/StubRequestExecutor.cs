using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations
{
   internal class StubRequestExecutor : IStubRequestExecutor
   {
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubContainer _stubContainer;

      public StubRequestExecutor(
         IServiceProvider serviceProvider,
         IStubContainer stubContainer)
      {
         _serviceProvider = serviceProvider;
         _stubContainer = stubContainer;
      }

      public async Task<ResponseModel> ExecuteRequestAsync()
      {
         string finalStubId = null;
         var conditionCheckers = (IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker));
         foreach (var checker in conditionCheckers)
         {
            (ConditionCheckResultType result, string stubId) = await checker.ValidateAsync();
            if (result == ConditionCheckResultType.Invalid)
            {
               // TODO return response prematurely
            }
            else if (result == ConditionCheckResultType.Valid && string.IsNullOrEmpty(stubId))
            {
               // TODO throw exception here; this isn't right.
            }

            if (finalStubId != null && stubId != null && finalStubId != stubId)
            {
               // TODO throw exception here; this isn't right.
            }

            finalStubId = stubId;
         }

         // Retrieve stub and parse response.
         var stub = _stubContainer.GetStubById(finalStubId);
         if (stub != null)
         {
            var response = new ResponseModel
            {
               StatusCode = 200
            };

            response.StatusCode = stub.Response?.StatusCode ?? 200;
            response.Body = stub.Response?.Text;

            var stubResponseHeaders = stub.Response?.Headers;
            if (stubResponseHeaders != null)
            {
               foreach (var header in stubResponseHeaders)
               {
                  response.Headers.Add(header.Key, header.Value);
               }
            }

            return response;
         }

         // TODO throw exception of zo
         throw new Exception();
      }
   }
}
