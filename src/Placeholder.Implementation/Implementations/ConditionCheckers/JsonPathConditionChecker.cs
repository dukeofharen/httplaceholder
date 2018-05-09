using System.Linq;
using Newtonsoft.Json.Linq;
using Placeholder.Implementation.Services;
using Placeholder.Models;
using Placeholder.Models.Enums;

namespace Placeholder.Implementation.Implementations.ConditionCheckers
{
   public class JsonPathConditionChecker : IConditionChecker
   {
      private readonly IRequestLoggerFactory _requestLoggerFactory;
      private readonly IHttpContextService _httpContextService;

      public JsonPathConditionChecker(
         IRequestLoggerFactory requestLoggerFactory,
         IHttpContextService httpContextService)
      {
         _requestLoggerFactory = requestLoggerFactory;
         _httpContextService = httpContextService;
      }

      public ConditionValidationType Validate(StubModel stub)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var result = ConditionValidationType.NotExecuted;
         var jsonPathConditions = stub.Conditions?.JsonPath?.ToArray();
         if (jsonPathConditions != null)
         {
            requestLogger.Log($"JSONPath condition found for stub '{stub.Id}': '{string.Join(", ", jsonPathConditions)}'");
            int validJsonPaths = 0;
            string body = _httpContextService.GetBody();
            var jsonObject = JObject.Parse(body);
            foreach (var condition in jsonPathConditions)
            {
               requestLogger.Log($"Checking posted content with JSONPath '{condition}'.");
               var elements = jsonObject.SelectToken(condition);
               if (elements == null || !elements.Any())
               {
                  // No suitable XML results found.
                  requestLogger.Log("No suitable JSON results found with JSONPath query.");
                  break;
               }

               validJsonPaths++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            if (validJsonPaths == jsonPathConditions.Length)
            {
               requestLogger.Log($"JSONPath condition check succeeded for stub '{stub.Id}'.");
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
