using System.Linq;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
    public class JsonPathConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public JsonPathConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var jsonPathConditions = conditions?.JsonPath?.ToArray();
            if (jsonPathConditions != null)
            {
                int validJsonPaths = 0;
                string body = _httpContextService.GetBody();
                var jsonObject = JObject.Parse(body);
                foreach (var condition in jsonPathConditions)
                {
                    var elements = jsonObject.SelectToken(condition);
                    if (elements == null)
                    {
                        // No suitable JSON results found.
                        result.Log = $"No suitable JSON results found with JSONPath query '{condition}'.";
                        break;
                    }

                    validJsonPaths++;
                }

                // If the number of succeeded conditions is equal to the actual number of conditions,
                // the header condition is passed and the stub ID is passed to the result.
                if (validJsonPaths == jsonPathConditions.Length)
                {
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
                else
                {
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
            }

            return result;
        }
    }
}