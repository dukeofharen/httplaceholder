using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class BodyConditionChecker : IConditionChecker
   {
      private readonly ILogger<BodyConditionChecker> _logger;
      private readonly IHttpContextService _httpContextService;
      private readonly IStubManager _stubContainer;

      public BodyConditionChecker(
         ILogger<BodyConditionChecker> logger,
         IHttpContextService httpContextService,
         IStubManager stubContainer)
      {
         _logger = logger;
         _httpContextService = httpContextService;
         _stubContainer = stubContainer;
      }

      public IEnumerable<string> Validate(IEnumerable<string> stubIds)
      {
         List<string> result = null;
         var stubs = _stubContainer.GetStubsByIds(stubIds);
         foreach (var stub in stubs)
         {
            var bodyConditions = stub.Conditions?.Body?.ToArray();
            if (bodyConditions != null)
            {
               var body = _httpContextService.GetBody();
               _logger.LogInformation($"Body condition found for stub '{stub.Id}': '{string.Join(", ", bodyConditions)}'");
               if (result == null)
               {
                  result = new List<string>();
               }

               int validBodyConditions = 0;

               foreach (var condition in bodyConditions)
               {
                  if (!StringHelper.IsRegexMatchOrSubstring(body, condition))
                  {
                     // If the check failed, it means the query string is incorrect and the condition should fail.
                     _logger.LogInformation($"Body condition '{condition}' failed.");
                     break;
                  }

                  validBodyConditions++;
               }

               // If the number of succeeded conditions is equal to the actual number of conditions,
               // the body condition is passed and the stub ID is passed to the result.
               if (validBodyConditions == bodyConditions.Length)
               {
                  _logger.LogInformation($"Body condition check succeeded for stub '{stub.Id}'.");
                  result.Add(stub.Id);
               }
            }
         }

         return result;
      }
   }
}
