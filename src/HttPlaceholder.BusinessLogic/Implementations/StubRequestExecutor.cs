using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Services;
using Microsoft.Extensions.DependencyInjection;
using HttPlaceholder.DataLogic;
using HttPlaceholder.Exceptions;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.BusinessLogic.Implementations
{
   public class StubRequestExecutor : IStubRequestExecutor
   {
      private readonly ILogger<StubRequestExecutor> _logger;
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IServiceProvider _serviceProvider;
      private readonly IStubContainer _stubContainer;
      private readonly IStubResponseGenerator _stubResponseGenerator;

      public StubRequestExecutor(
         ILogger<StubRequestExecutor> logger,
         IRequestLoggerFactory requestLoggerFactory,
         IServiceProvider serviceProvider,
         IStubContainer stubContainer,
         IStubResponseGenerator stubResponseGenerator)
      {
         _logger = logger;
         _requestLoggerFactory = requestLoggerFactory;
         _serviceProvider = serviceProvider;
         _stubContainer = stubContainer;
         _stubResponseGenerator = stubResponseGenerator;
      }

      public async Task<ResponseModel> ExecuteRequestAsync()
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var conditionCheckers = ((IEnumerable<IConditionChecker>)_serviceProvider.GetServices(typeof(IConditionChecker))).ToArray();

         var foundStubs = new List<StubModel>();
         var stubs = await _stubContainer.GetStubsAsync();

         foreach (var stub in stubs)
         {
            try
            {
               bool passed = false;
               var validationResults = new List<ConditionCheckResultModel>();
               var negativeValidationResults = new List<ConditionCheckResultModel>();
               foreach (var checker in conditionCheckers)
               {
                  ConditionCheckResultModel result;

                  // First, check the regular conditions.
                  result = CheckConditions(stub.Id, checker, stub.Conditions, false);
                  validationResults.Add(result);

                  // Then check the "negative" conditions. These conditions are the "not" scenarios.
                  result = CheckConditions(stub.Id, checker, stub.NegativeConditions, true);
                  negativeValidationResults.Add(result);
               }

               var allValidationResults = validationResults.Concat(negativeValidationResults);
               if (allValidationResults.All(r => r.ConditionValidation != ConditionValidationType.Invalid) && validationResults.Any(r => r.ConditionValidation != ConditionValidationType.NotExecuted && r.ConditionValidation != ConditionValidationType.NotSet))
               {
                  passed = true;
                  foundStubs.Add(stub);
               }

               requestLogger.SetStubExecutionResult(stub.Id, passed, validationResults, negativeValidationResults);
            }
            catch (Exception e)
            {
               _logger.LogWarning($"Exception thrown while executing checks for stub '{stub.Id}': {e}");
            }
         }

         if (!foundStubs.Any())
         {
            // If the resulting list is not null, but empty, the condition did not pass and the response should be returned prematurely.
            throw new RequestValidationException($"The '{nameof(foundStubs)}' array for condition was empty, which means the condition was configured and the request did not pass or no conditions are configured at all.");
         }

         var finalStub = foundStubs.First();
         requestLogger.SetExecutingStubId(finalStub.Id);
         var response = await _stubResponseGenerator.GenerateResponseAsync(finalStub);
         return response;
      }

      private ConditionCheckResultModel CheckConditions(string stubId, IConditionChecker checker, StubConditionsModel conditions, bool negative)
      {
         var validationResult = checker.Validate(stubId, conditions);
         validationResult.CheckerName = checker.GetType().Name;
         var conditionValidation = validationResult.ConditionValidation;
         if (negative)
         {
            if (conditionValidation != ConditionValidationType.NotExecuted)
            {
               validationResult.ConditionValidation = conditionValidation == ConditionValidationType.Invalid ? ConditionValidationType.Valid : ConditionValidationType.Invalid;
            }
         }

         return validationResult;
      }
   }
}
