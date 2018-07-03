using System.Linq;
using HttPlaceholder.Services;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Utilities;

namespace HttPlaceholder.Implementation.Implementations.ConditionCheckers
{
   public class BodyConditionChecker : IConditionChecker
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IHttpContextService _httpContextService;

      public BodyConditionChecker(
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
         var bodyConditions = conditions?.Body?.ToArray();
         if (bodyConditions != null)
         {
            var body = _httpContextService.GetBody();
            requestLogger.Log($"Body condition found for stub '{stubId}': '{string.Join(", ", bodyConditions)}'");

            int validBodyConditions = 0;
            foreach (var condition in bodyConditions)
            {
               if (!StringHelper.IsRegexMatchOrSubstring(body, condition))
               {
                  // If the check failed, it means the query string is incorrect and the condition should fail.
                  requestLogger.Log($"Body condition '{condition}' failed.");
                  break;
               }

               validBodyConditions++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the body condition is passed and the stub ID is passed to the result.
            if (validBodyConditions == bodyConditions.Length)
            {
               requestLogger.Log($"Body condition check succeeded for stub '{stubId}'.");
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
