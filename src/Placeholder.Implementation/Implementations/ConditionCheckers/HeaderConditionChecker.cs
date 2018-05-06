using System.Linq;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class HeaderConditionChecker : IConditionChecker
   {
      private readonly ILogger<HeaderConditionChecker> _logger;
      private readonly IHttpContextService _httpContextService;

      public HeaderConditionChecker(
         ILogger<HeaderConditionChecker> logger,
         IHttpContextService httpContextService)
      {
         _logger = logger;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(StubModel stub)
      {
         var result = ConditionValidationType.NotExecuted;
         var headerConditions = stub.Conditions?.Headers;
         if (headerConditions != null)
         {
            _logger.LogInformation($"Headers condition found for stub '{stub.Id}': '{string.Join(", ", headerConditions.Select(c => $"{c.Key}: {c.Value}"))}'");
            int validHeaders = 0;
            var headers = _httpContextService.GetHeaders();
            foreach (var condition in headerConditions)
            {
               // Check whether the condition header is available in the actual headers.
               _logger.LogInformation($"Checking request headers against headers condition '{condition.Key}: {condition.Value}'");
               if (headers.TryGetValue(condition.Key, out string headerValue))
               {
                  // Check whether the condition header value is available in the actual headers.
                  string value = condition.Value ?? string.Empty;
                  if (!StringHelper.IsRegexMatchOrSubstring(headerValue, value))
                  {
                     // If the check failed, it means the header is incorrect and the condition should fail.
                     _logger.LogInformation($"Header condition '{condition.Key}: {condition.Value}' failed.");
                     break;
                  }

                  validHeaders++;
               }
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            if (validHeaders == headerConditions.Count)
            {
               _logger.LogInformation($"Header condition check succeeded for stub '{stub.Id}'.");
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
