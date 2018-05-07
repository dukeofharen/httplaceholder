using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations
{
   public class StubRequestExecutor : IStubRequestExecutor
   {
      private readonly ILogger<StubRequestExecutor> _logger;
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubManager _stubManager;

      public StubRequestExecutor(
         ILogger<StubRequestExecutor> logger,
         IServiceProvider serviceProvider,
         IStubManager stubManager)
      {
         _logger = logger;
         _serviceProvider = serviceProvider;
         _stubManager = stubManager;
      }

      public ResponseModel ExecuteRequest()
      {
         var conditionCheckers = ((IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker))).ToArray();
         _logger.LogInformation($"Following conditions found: {string.Join(", ", conditionCheckers.Select(c => c.GetType().ToString()))}");

         var stubIds = new List<string>();
         var stubs = _stubManager.Stubs;
         foreach (var stub in stubs)
         {
            try
            {
               var validationResults = new List<ConditionValidationType>();
               _logger.LogInformation($"Validating conditions for stub '{stub.Id}'.");
               foreach (var checker in conditionCheckers)
               {
                  _logger.LogInformation($"Executing condition '{checker.GetType()}'.");
                  var validationResult = checker.Validate(stub);
                  validationResults.Add(validationResult);
                  if (validationResult == ConditionValidationType.NotExecuted)
                  {
                     // If the resulting list is null, it means the check wasn't executed because it wasn't configured. Continue with the next condition.
                     _logger.LogInformation("The condition was not executed and not configured.");
                     continue;
                  }

                  if (validationResult == ConditionValidationType.Invalid)
                  {
                     _logger.LogInformation("The condition did not pass.");
                  }
               }

               if (validationResults.All(r => r != ConditionValidationType.Invalid) && validationResults.Any(r => r != ConditionValidationType.NotExecuted && r != ConditionValidationType.NotSet))
               {
                  _logger.LogInformation($"All conditions passed for stub '{stub.Id}'.");
                  stubIds.Add(stub.Id);
               }
            }
            catch (Exception e)
            {
               _logger.LogInformation($"Exception thrown while executing condition checks for stub '{stub.Id}': {e}");
            }
         }

         if (!stubIds.Any())
         {
            // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
            throw new Exception($"The '{nameof(stubIds)}' array for condition was empty, which means the condition was configured and the request did not pass or no conditions are configured at all.");
         }

         if (stubIds.Count > 1)
         {
            // Multiple conditions found; don't know which one to choose. Throw an exception.
            throw new Exception($"'{nameof(stubIds)}' contains '{stubIds.Count}' stub IDs, which means no choice can be made.");
         }

         string finalStubId = stubIds.Single();

         // Retrieve stub and parse response.
         var finalStub = _stubManager.GetStubById(finalStubId);
         if (finalStub != null)
         {
            _logger.LogInformation($"Stub with ID '{finalStubId}' found; returning response.");
            var response = new ResponseModel
            {
               StatusCode = 200
            };

            response.StatusCode = finalStub.Response?.StatusCode ?? 200;
            _logger.LogInformation($"Found HTTP status code '{response.StatusCode}'.");

            response.Body = finalStub.Response?.Text;
            _logger.LogInformation($"Found body '{response.Body}'");

            var stubResponseHeaders = finalStub.Response?.Headers;
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
