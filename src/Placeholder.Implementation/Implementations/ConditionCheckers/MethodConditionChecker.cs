using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class MethodConditionChecker : IConditionChecker
   {
      private readonly ILogger<MethodConditionChecker> _logger;
      private readonly IHttpContextService _httpContextService;

      public MethodConditionChecker(
         ILogger<MethodConditionChecker> logger,
         IHttpContextService httpContextService)
      {
         _logger = logger;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(StubModel stub)
      {
         var result = ConditionValidationType.NotExecuted;
         string methodCondition = stub.Conditions?.Method;
         if (!string.IsNullOrEmpty(methodCondition))
         {
            _logger.LogInformation($"Method condition found for stub '{stub.Id}': '{methodCondition}'");

            string method = _httpContextService.Method;
            if (string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase))
            {
               // The path matches the provided regex. Add the stub ID to the resulting list.
               _logger.LogInformation($"Condition '{methodCondition}' passed for request.");
               result = ConditionValidationType.Valid;
            }
            else
            {
               _logger.LogInformation($"Condition '{methodCondition}' did not pass for request.");
               result = ConditionValidationType.Invalid;
            }
         }

         return result;
      }
   }
}
