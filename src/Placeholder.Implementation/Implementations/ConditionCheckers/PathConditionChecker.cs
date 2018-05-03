using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Placeholder.Models.Enums;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   internal class PathConditionChecker : IConditionChecker
   {
      private readonly IHttpContextAccessor _httpContextAccessor;
      private readonly IStubContainer _stubContainer;

      public PathConditionChecker(
         IHttpContextAccessor httpContextAccessor,
         IStubContainer stubContainer)
      {
         _httpContextAccessor = httpContextAccessor;
         _stubContainer = stubContainer;
      }

      public Task<IEnumerable<string>> ValidateAsync(IEnumerable<string> stubIds)
      {
         List<string> result = null;
         var stubs = stubIds == null ? _stubContainer.Stubs : _stubContainer.GetStubsByIds(stubIds);
         foreach (var stub in stubs)
         {
            string pathCondition = stub.Conditions?.Url?.Path;
            if (!string.IsNullOrEmpty(pathCondition))
            {
               if (result == null)
               {
                  result = new List<string>();
               }

               string path = _httpContextAccessor.HttpContext.Request.Path.ToString();
               if (StringHelper.IsRegexMatchOrSubstring(path, pathCondition))
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
