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

      public Task<(ConditionCheckResultType, string)> ValidateAsync()
      {
         var result = ConditionCheckResultType.NotExecuted;
         string id = null;

         // First, find the correct stub entry.
         var stubs = _stubContainer.Stubs;
         foreach (var stub in stubs)
         {
            if (stub is IDictionary<object, object> stubEntries)
            {
               id = stubEntries.FirstOrDefault(e => e.Key.ToString() == "id").Value?.ToString();
               var conditions = stubEntries.FirstOrDefault(e => e.Key.ToString() == "conditions").Value as IDictionary<object, object>;
               var urlConditions = conditions.FirstOrDefault(c => c.Key.ToString() == "url").Value as IDictionary<object, object>;
               string pathCondition = urlConditions.FirstOrDefault(c => c.Key.ToString() == "path").Value?.ToString();
               if (string.IsNullOrEmpty(pathCondition))
               {
                  // The condition is not found for this stub. Continue looking in the next stub.
               }
               else
               {
                  if (!string.IsNullOrEmpty(pathCondition))
                  {
                     string path = _httpContextAccessor.HttpContext.Request.Path.ToString();
                     if (!StringHelper.IsRegexMatchOrSubstring(path, pathCondition))
                     {
                        // The path doesn't match the provided regex. Continue looking in the next stub.
                        result = ConditionCheckResultType.Invalid;
                     }
                     else
                     {
                        // The path matches the provided regex. Break the loop and return the result.
                        result = ConditionCheckResultType.Valid;
                        break;
                     }
                  }
               }
            }
         }

         return Task.FromResult((result, id));
      }
   }
}
