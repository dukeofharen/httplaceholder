using System.Linq;
using HttPlaceholder.Services;
using Newtonsoft.Json.Linq;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;

namespace HttPlaceholder.Implementation.Implementations.ConditionCheckers
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

      public ConditionValidationType Validate(string stubId, StubConditionsModel conditions)
      {
         var requestLogger = _requestLoggerFactory.GetRequestLogger();
         var result = ConditionValidationType.NotExecuted;
         var jsonPathConditions = conditions?.JsonPath?.ToArray();
         if (jsonPathConditions != null)
         {
            requestLogger.Log($"JSONPath condition found for stub '{stubId}': '{string.Join(", ", jsonPathConditions)}'");
            int validJsonPaths = 0;
            string body = _httpContextService.GetBody();
            var jsonObject = JObject.Parse(body);
            foreach (var condition in jsonPathConditions)
            {
               requestLogger.Log($"Checking posted content with JSONPath '{condition}'.");
               var elements = jsonObject.SelectToken(condition);
               if (elements == null)
               {
                  // No suitable JSON results found.
                  requestLogger.Log("No suitable JSON results found with JSONPath query.");
                  break;
               }

               validJsonPaths++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            if (validJsonPaths == jsonPathConditions.Length)
            {
               requestLogger.Log($"JSONPath condition check succeeded for stub '{stubId}'.");
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
