using System.Linq;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
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

      public ConditionValidationType Validate(StubModel stub)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var result = ConditionValidationType.NotExecuted;
         var bodyConditions = stub.Conditions?.Body?.ToArray();
         if (bodyConditions != null)
         {
            var body = _httpContextService.GetBody();
            requestLogger.Log($"Body condition found for stub '{stub.Id}': '{string.Join(", ", bodyConditions)}'");

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
               requestLogger.Log($"Body condition check succeeded for stub '{stub.Id}'.");
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
