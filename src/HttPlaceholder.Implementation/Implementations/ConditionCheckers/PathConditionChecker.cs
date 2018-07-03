using HttPlaceholder.Services;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Utilities;

namespace HttPlaceholder.Implementation.Implementations.ConditionCheckers
{
   public class PathConditionChecker : IConditionChecker
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IHttpContextService _httpContextService;

      public PathConditionChecker(
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
         string pathCondition = conditions?.Url?.Path;
         if (!string.IsNullOrEmpty(pathCondition))
         {
            requestLogger.Log($"Path condition found for stub '{stubId}': '{pathCondition}'");
            string path = _httpContextService.Path;
            if (StringHelper.IsRegexMatchOrSubstring(path, pathCondition))
            {
               // The path matches the provided regex. Add the stub ID to the resulting list.
               requestLogger.Log($"Condition '{pathCondition}' passed for request.");
               result = ConditionValidationType.Valid;
            }
            else
            {
               requestLogger.Log($"Condition '{pathCondition}' did not pass for request.");
               result = ConditionValidationType.Invalid;
            }
         }

         return result;
      }
   }
}
