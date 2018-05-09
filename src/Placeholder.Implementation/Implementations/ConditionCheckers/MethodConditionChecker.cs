using System;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class MethodConditionChecker : IConditionChecker
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IHttpContextService _httpContextService;

      public MethodConditionChecker(
         IRequestLoggerFactory requestLoggerFactory,
         IHttpContextService httpContextService)
      {
         _requestLoggerFactory = requestLoggerFactory;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(StubModel stub)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var result = ConditionValidationType.NotExecuted;
         string methodCondition = stub.Conditions?.Method;
         if (!string.IsNullOrEmpty(methodCondition))
         {
            requestLogger.Log($"Method condition found for stub '{stub.Id}': '{methodCondition}'");
            string method = _httpContextService.Method;
            if (string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase))
            {
               // The path matches the provided regex. Add the stub ID to the resulting list.
               requestLogger.Log($"Condition '{methodCondition}' passed for request.");
               result = ConditionValidationType.Valid;
            }
            else
            {
               requestLogger.Log($"Condition '{methodCondition}' did not pass for request.");
               result = ConditionValidationType.Invalid;
            }
         }

         return result;
      }
   }
}
