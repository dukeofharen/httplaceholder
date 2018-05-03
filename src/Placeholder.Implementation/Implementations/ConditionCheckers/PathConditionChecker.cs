using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Placeholder.Implementation.Services;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   internal class PathConditionChecker : IConditionChecker
   {
      private readonly IHttpContextService _httpContextService;
      private readonly IStubContainer _stubContainer;

      public PathConditionChecker(
         IHttpContextService httpContextService,
         IStubContainer stubContainer)
      {
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
               if (result == null)
               {
                  result = new List<string>();
               }

               string path = _httpContextService.Path;
               if (StringHelper.IsRegexMatchOrSubstring(path, pathCondition))
               {
                  // The path matches the provided regex. Add the stub ID to the resulting list.
                  result.Add(stub.Id);
               }
            }
         }

         return Task.FromResult(result?.AsEnumerable());
      }
   }
}
