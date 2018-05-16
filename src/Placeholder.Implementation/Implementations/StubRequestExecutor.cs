using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Exceptions;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations
{
   public class StubRequestExecutor : IStubRequestExecutor
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubManager _stubManager;
      private readonly IStubResponseGenerator _stubResponseGenerator;

      public StubRequestExecutor(
         IRequestLoggerFactory requestLoggerFactory,
         IServiceProvider serviceProvider,
         IStubManager stubManager,
         IStubResponseGenerator stubResponseGenerator)
      {
         _requestLoggerFactory = requestLoggerFactory;
         _serviceProvider = serviceProvider;
         _stubManager = stubManager;
         _stubResponseGenerator = stubResponseGenerator;
      }

      public ResponseModel ExecuteRequest()
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var conditionCheckers = ((IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker))).ToArray();
         requestLogger.Log($"Following conditions found: {string.Join(", ", conditionCheckers.Select(c => c.GetType().ToString()))}");

         var foundStubs = new List<StubModel>();
         var stubs = _stubManager.Stubs;

         foreach (var stub in stubs)
         {
            requestLogger.Log($"-------- CHECKING {stub.Id} --------");
            try
            {
               var validationResults = new List<ConditionValidationType>();
               foreach (var checker in conditionCheckers)
               {
                  requestLogger.Log($"----- EXECUTING CONDITION {checker.GetType().Name} -----");
                  var validationResult = checker.Validate(stub);
                  validationResults.Add(validationResult);
                  if (validationResult == ConditionValidationType.NotExecuted)
                  {
                     // If the resulting list is null, it means the check wasn't executed because it wasn't configured. Continue with the next condition.
                     requestLogger.Log("The condition was not executed and not configured.");
                     continue;
                  }

                  if (validationResult == ConditionValidationType.Invalid)
                  {
                     requestLogger.Log("The condition did not pass.");
                  }

                  requestLogger.Log($"----- DONE EXECUTING CONDITION {checker.GetType().Name} -----");
               }

               if (validationResults.All(r => r != ConditionValidationType.Invalid) && validationResults.Any(r => r != ConditionValidationType.NotExecuted && r != ConditionValidationType.NotSet))
               {
                  requestLogger.Log($"All conditions passed for stub '{stub.Id}'.");
                  foundStubs.Add(stub);
               }
            }
            catch (Exception e)
            {
               requestLogger.Log($"Exception thrown while executing condition checks for stub '{stub.Id}': {e}");
            }

            requestLogger.Log($"-------- DONE CHECKING {stub.Id} --------");
         }

         if (!foundStubs.Any())
         {
            // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
            throw new RequestValidationException($"The '{nameof(foundStubs)}' array for condition was empty, which means the condition was configured and the request did not pass or no conditions are configured at all.");
         }

         if (stubIds.Count > 1)
         {
            requestLogger.Log($"Multiple stubs are found ({string.Join(", ", stubIds)}), picking the first one.");
         }
         if (foundStubs.Count > 1)
         {
            requestLogger.Log($"Multiple stubs are found ({string.Join(", ", foundStubs)}), picking the first one.");
         }

         var finalStub = foundStubs.First();
         var response = _stubResponseGenerator.GenerateResponse(finalStub);
         return response;
      }
   }
}
