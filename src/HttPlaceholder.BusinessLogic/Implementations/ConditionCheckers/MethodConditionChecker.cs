using System;
using HttPlaceholder.Services;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
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

      public ConditionValidationType Validate(string stubId, StubConditionsModel conditions)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var result = ConditionValidationType.NotExecuted;
         string methodCondition = conditions?.Method;
         if (!string.IsNullOrEmpty(methodCondition))
         {
            requestLogger.Log($"Method condition found for stub '{stubId}': '{methodCondition}'");
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
