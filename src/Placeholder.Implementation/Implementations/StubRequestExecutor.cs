using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Budgetkar.Services;
using Microsoft.Extensions.DependencyInjection;
using Placeholder.Exceptions;
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

      public async Task<ResponseModel> ExecuteRequestAsync()
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
                  ConditionValidationType result;

                  // First, check the regular conditions.
                  result = CheckConditions(stub.Id, checker, stub.Conditions, false);
                  validationResults.Add(result);

                  // Then check the "negative" conditions. These conditions are the "not" scenarios.
                  result = CheckConditions(stub.Id, checker, stub.NegativeConditions, true);
                  validationResults.Add(result);
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

         if (foundStubs.Count > 1)
         {
            requestLogger.Log($"Multiple stubs are found ({string.Join(", ", foundStubs)}), picking the first one.");
         }

         var finalStub = foundStubs.First();
         var response = await _stubResponseGenerator.GenerateResponseAsync(finalStub);
         return response;
      }

      private ConditionValidationType CheckConditions(string stubId, IConditionChecker checker, StubConditionsModel conditions, bool negative)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         requestLogger.Log($"----- EXECUTING{(negative ? " NEGATIVE " : " ")}CONDITION {checker.GetType().Name} -----");
         var validationResult = checker.Validate(stubId, conditions);
         if (validationResult == ConditionValidationType.NotExecuted)
         {
            // If the resulting list is null, it means the check wasn't executed because it wasn't configured. Continue with the next condition.
            requestLogger.Log("The condition was not executed and not configured.");
         }

         if (validationResult == ConditionValidationType.Invalid)
         {
            requestLogger.Log("The condition did not pass.");
         }

         requestLogger.Log($"----- DONE EXECUTING{(negative ? " NEGATIVE " : " ")}CONDITION {checker.GetType().Name} -----");

         if (negative)
         {
            if (validationResult == ConditionValidationType.NotExecuted)
            {
               return ConditionValidationType.NotExecuted;
            }

            return validationResult == ConditionValidationType.Invalid ? ConditionValidationType.Valid : ConditionValidationType.Invalid;
         }

         return validationResult;
      }
   }
}
