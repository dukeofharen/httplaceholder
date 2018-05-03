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
            string pathCondition = stub.Conditions?.Url?.Path;
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
                     id = stub.Id;
                     result = ConditionCheckResultType.Valid;
                     break;
                  }
               }
            }
         }

         return Task.FromResult((result, id));
      }
   }
}
