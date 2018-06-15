using System;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
    public class FullPathConditionChecker : IConditionChecker
    {
       private readonly IRequestLoggerFactory _requestLoggerFactory;
       private readonly IHttpContextService _httpContextService;

       public FullPathConditionChecker(
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
          string fullPathCondition = conditions?.Url?.FullPath;
          if (!string.IsNullOrEmpty(fullPathCondition))
          {
             requestLogger.Log($"Full path condition found for stub '{stubId}': '{fullPathCondition}'");
             string path = _httpContextService.FullPath;
             if (StringHelper.IsRegexMatchOrSubstring(path, fullPathCondition))
             {
                // The path matches the provided regex. Add the stub ID to the resulting list.
                requestLogger.Log($"Condition '{fullPathCondition}' passed for request.");
                result = ConditionValidationType.Valid;
             }
             else
             {
                requestLogger.Log($"Condition '{fullPathCondition}' did not pass for request.");
                result = ConditionValidationType.Invalid;
             }
          }

          return result;
      }
    }
}
