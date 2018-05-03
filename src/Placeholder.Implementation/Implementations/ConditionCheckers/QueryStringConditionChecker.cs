using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Placeholder.Implementation.Services;
using Placeholder.Utilities;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   internal class QueryStringConditionChecker : IConditionChecker
   {
      private readonly IHttpContextService _httpContextService;
      private readonly IStubContainer _stubContainer;

      public QueryStringConditionChecker(
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
            var queryStringConditions = stub.Conditions?.Url?.Query;
            if (queryStringConditions != null)
            {
               if (result == null)
               {
                  result = new List<string>();
               }

               int validQueryStrings = 0;
               var queryString = _httpContextService.GetQueryStringDictionary();
               foreach (var condition in queryStringConditions)
               {
                  // Check whether the condition query is available in the actual query string.
                  if (queryString.TryGetValue(condition.Key, out string queryValue))
                  {
                     // Check whether the condition query value is available in the actual query string.
                     string value = condition.Value ?? string.Empty;
                     if (!StringHelper.IsRegexMatchOrSubstring(value, queryValue))
                     {
                        // If the check failed, it means the query string is incorrect and the condition should fail.
                        break;
                     }

                     validQueryStrings++;
                  }
               }

               // If the number of succeeded conditions is equal to the actual number of conditions,
               // the query string condition is passed and the stub ID is passed to the result.
               if (validQueryStrings == queryStringConditions.Count)
               {
                  result.Add(stub.Id);
               }
            }
         }

         return Task.FromResult(result?.AsEnumerable());
      }
   }
}
