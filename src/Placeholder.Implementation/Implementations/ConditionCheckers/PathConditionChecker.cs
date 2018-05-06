using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class PathConditionChecker : IConditionChecker
   {
      private readonly ILogger<PathConditionChecker> _logger;
      private readonly IHttpContextService _httpContextService;

      public PathConditionChecker(
         ILogger<PathConditionChecker> logger,
         IHttpContextService httpContextService)
      {
         _logger = logger;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(StubModel stub)
      {
         var result = ConditionValidationType.NotExecuted;
         string pathCondition = stub.Conditions?.Url?.Path;
         if (!string.IsNullOrEmpty(pathCondition))
         {
            _logger.LogInformation($"Path condition found for stub '{stub.Id}': '{pathCondition}'");
            string path = _httpContextService.Path;
            if (StringHelper.IsRegexMatchOrSubstring(path, pathCondition))
            {
               // The path matches the provided regex. Add the stub ID to the resulting list.
               _logger.LogInformation($"Condition '{pathCondition}' passed for request.");
               result = ConditionValidationType.Valid;
            }
            else
            {
               _logger.LogInformation($"Condition '{pathCondition}' did not pass for request.");
               result = ConditionValidationType.Invalid;
            }
         }

         return result;
      }
   }
}
