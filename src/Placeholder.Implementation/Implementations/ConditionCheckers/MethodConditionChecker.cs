using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Placeholder.Implementation.Services;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   internal class MethodConditionChecker : IConditionChecker
   {
      private readonly IHttpContextService _httpContextService;
      private readonly IStubContainer _stubContainer;

      public MethodConditionChecker(
         IHttpContextService httpContextService,
         IStubContainer stubContainer)
      {
         _httpContextService = httpContextService;
         _stubContainer = stubContainer;
      }

      public Task<IEnumerable<string>> ValidateAsync(IEnumerable<string> stubIds)
      {
         List<string> result = null;
         var stubs = stubIds == null ? _stubContainer.Stubs : _stubContainer.GetStubsByIds(stubIds);
         foreach (var stub in stubs)
         {
            string methodCondition = stub.Conditions?.Method;
            if (!string.IsNullOrEmpty(methodCondition))
            {
               if (result == null)
               {
                  result = new List<string>();
               }

               string method = _httpContextService.Method;
               if (string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase))
               {
                  // The path matches the provided regex. Add the stub ID to the resulting list.
                  result.Add(stub.Id);
               }
            }
         }

         return Task.FromResult(result.AsEnumerable());
      }
   }
}
