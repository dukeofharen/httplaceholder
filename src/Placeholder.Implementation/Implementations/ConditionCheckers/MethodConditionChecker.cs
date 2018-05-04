using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Placeholder.Implementation.Services;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class MethodConditionChecker : IConditionChecker
   {
      private readonly ILogger<MethodConditionChecker> _logger;
      private readonly IHttpContextService _httpContextService;
      private readonly IStubManager _stubContainer;

      public MethodConditionChecker(
         ILogger<MethodConditionChecker> logger,
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
            string methodCondition = stub.Conditions?.Method;
            if (!string.IsNullOrEmpty(methodCondition))
            {
               _logger.LogInformation($"Method condition found for stub '{stub.Id}': '{methodCondition}'");
               if (result == null)
               {
                  result = new List<string>();
               }

               string method = _httpContextService.Method;
               if (string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase))
               {
                  // The path matches the provided regex. Add the stub ID to the resulting list.
                  _logger.LogInformation($"Condition '{methodCondition}' passed for request.");
                  result.Add(stub.Id);
               }
               else
               {
                  _logger.LogInformation($"Condition '{methodCondition}' did not pass for request.");
               }
            }
         }

         return Task.FromResult(result?.AsEnumerable());
      }
   }
}
