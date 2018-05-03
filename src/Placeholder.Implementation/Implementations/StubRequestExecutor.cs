using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Models;

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
         var conditionCheckers = (IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker));
         string[] stubIds = null;
         foreach (var checker in conditionCheckers)
         {
            stubIds = (await checker.ValidateAsync(stubIds)).ToArray();
            if (stubIds == null)
            {
               // If the resulting list is null, it means the check wasn't executed because it wasn't configured. Continue with the next condition.
               continue;
            }

            if (!stubIds.Any())
            {
               // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
               // TODO sensible exception error
               throw new Exception();
            }
         }

         if (stubIds == null)
         {
            // No conditions passed or no conditions configured. Throw an exception.
            // TODO sensible exception error
            throw new Exception();
         }

         if (stubIds.Length > 1)
         {
            // Multiple conditions found; don't know which one to choose. Throw an exception.
            // TODO sensible exception error
            throw new Exception();
         }

         string finalStubId = stubIds.Single();

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
