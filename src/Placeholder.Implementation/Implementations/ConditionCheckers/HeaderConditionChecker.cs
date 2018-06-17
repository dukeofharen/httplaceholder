using System.Linq;
using Budgetkar.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class HeaderConditionChecker : IConditionChecker
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IHttpContextService _httpContextService;

      public HeaderConditionChecker(
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
         var headerConditions = conditions?.Headers;
         if (headerConditions != null)
         {
            requestLogger.Log($"Headers condition found for stub '{stubId}': '{string.Join(", ", headerConditions.Select(c => $"{c.Key}: {c.Value}"))}'");
            int validHeaders = 0;
            var headers = _httpContextService.GetHeaders();
            foreach (var condition in headerConditions)
            {
               // Check whether the condition header is available in the actual headers.
               requestLogger.Log($"Checking request headers against headers condition '{condition.Key}: {condition.Value}'");
               if (headers.TryGetValue(condition.Key, out string headerValue))
               {
                  // Check whether the condition header value is available in the actual headers.
                  string value = condition.Value ?? string.Empty;
                  if (!StringHelper.IsRegexMatchOrSubstring(headerValue, value))
                  {
                     // If the check failed, it means the header is incorrect and the condition should fail.
                     requestLogger.Log($"Header condition '{condition.Key}: {condition.Value}' failed.");
                     break;
                  }

                  validHeaders++;
               }
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            if (validHeaders == headerConditions.Count)
            {
               requestLogger.Log($"Header condition check succeeded for stub '{stubId}'.");
               result = ConditionValidationType.Valid;
            }
            else
            {
               result = ConditionValidationType.Invalid;
            }
         }

         return result;
      }
   }
}
