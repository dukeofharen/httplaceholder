using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   internal class PathConditionChecker : IConditionChecker
   {
      private readonly ILogger<PathConditionChecker> _logger;
      private readonly IHttpContextService _httpContextService;
      private readonly IStubManager _stubContainer;

      public PathConditionChecker(
         ILogger<PathConditionChecker> logger,
         IHttpContextService httpContextService,
         IStubManager stubContainer)
      {
         _logger = logger;
         _httpContextService = httpContextService;
         _stubContainer = stubContainer;
      }

      public Task<IEnumerable<string>> ValidateAsync(IEnumerable<string> stubIds)
      {
         List<string> result = null;
         var stubs = _stubContainer.GetStubsByIds(stubIds);
         foreach (var stub in stubs)
         {
            string pathCondition = stub.Conditions?.Url?.Path;
            if (!string.IsNullOrEmpty(pathCondition))
            {
               _logger.LogInformation($"Path condition found for stub '{stub.Id}': '{pathCondition}'");
               if (result == null)
               {
                  result = new List<string>();
               }

               string path = _httpContextService.Path;
               if (StringHelper.IsRegexMatchOrSubstring(path, pathCondition))
               {
                  // The path matches the provided regex. Add the stub ID to the resulting list.
                  _logger.LogInformation($"Condition '{pathCondition}' passed for request.");
                  result.Add(stub.Id);
               }
               else
               {
                  _logger.LogInformation($"Condition '{pathCondition}' did not pass for request.");
               }
            }
         }

         return Task.FromResult(result?.AsEnumerable());
      }
   }
}
