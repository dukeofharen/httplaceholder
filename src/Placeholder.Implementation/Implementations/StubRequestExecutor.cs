using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Placeholder.Models;

namespace Placeholder.Implementation.Implementations
{
   internal class StubRequestExecutor : IStubRequestExecutor
   {
      private readonly ILogger<StubRequestExecutor> _logger;
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubManager _stubContainer;

      public StubRequestExecutor(
         ILogger<StubRequestExecutor> logger,
         IServiceProvider serviceProvider,
         IStubManager stubContainer)
      {
         _logger = logger;
         _serviceProvider = serviceProvider;
         _stubContainer = stubContainer;
      }

      public ResponseModel ExecuteRequest()
      {
         var conditionCheckers = ((IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker))).ToArray();
         _logger.LogInformation($"Following conditions found: {string.Join(", ", conditionCheckers.Select(c => c.GetType().ToString()))}");

         string[] stubIds = null;
         foreach (var checker in conditionCheckers)
         {
            _logger.LogInformation($"Checking request with condition '{checker.GetType()}'.");
            var validationResult = checker.Validate(stubIds)?.ToArray();
            if (validationResult == null)
            {
               // If the resulting list is null, it means the check wasn't executed because it wasn't configured. Continue with the next condition.
               _logger.LogInformation($"'{nameof(stubIds)}' array for condition '{checker.GetType()}' was null, which means the condition was not executed and not configured.");
               continue;
            }

            stubIds = validationResult;
            if (!stubIds.Any())
            {
               // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
               throw new Exception($"The '{nameof(stubIds)}' array for condition was empty, which means the condition was configured but the request did not pass.");
            }
         }

         if (stubIds == null)
         {
            // No conditions passed or no conditions configured. Throw an exception.
            throw new Exception($"'{nameof(stubIds)}' was null, which means no condition was passed or no conditions are configured.");
         }

         if (stubIds.Length > 1)
         {
            // Multiple conditions found; don't know which one to choose. Throw an exception.
            throw new Exception($"'{nameof(stubIds)}' contains '{stubIds.Length}' stub IDs, which means no choice can be made.");
         }

         string finalStubId = stubIds.Single();

         // Retrieve stub and parse response.
         var stub = _stubContainer.GetStubById(finalStubId);
         if (stub != null)
         {
            _logger.LogInformation($"Stub with ID '{finalStubId}' found; returning response.");
            var response = new ResponseModel
            {
               StatusCode = 200
            };

            response.StatusCode = stub.Response?.StatusCode ?? 200;
            _logger.LogInformation($"Found HTTP status code '{response.StatusCode}'.");

            response.Body = stub.Response?.Text;
            _logger.LogInformation($"Found body '{response.Body}'");

            var stubResponseHeaders = stub.Response?.Headers;
            if (stubResponseHeaders != null)
            {
               foreach (var header in stubResponseHeaders)
               {
                  _logger.LogInformation($"Found header '{header.Key}' with value '{header.Value}'.");
                  response.Headers.Add(header.Key, header.Value);
               }
            }

            return response;
         }

         throw new Exception($"Stub with ID '{finalStubId}' unexpectedly not found.");
      }
   }
}
