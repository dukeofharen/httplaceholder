using System;
using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
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
            if (jsonPathConditions == null || jsonPathConditions?.Any() != true)
            {
                return result;
            }

            var validJsonPaths = 0;
            var body = _httpContextService.GetBody();
            var jsonObject = JObject.Parse(body);
            foreach (var condition in jsonPathConditions)
            {
                if (condition is string conditionString)
                {
                    var elements = jsonObject.SelectToken(conditionString);
                    if (elements == null)
                    {
                        // No suitable JSON results found.
                        result.Log = $"No suitable JSON results found with JSONPath query '{condition}'.";
                        break;
                    }

                    validJsonPaths++;
                }
                else
                {
                    StubJsonPathModel jsonPathCondition;
                    if (condition is JObject conditionObject)
                    {
                        jsonPathCondition = conditionObject.ToObject<StubJsonPathModel>();
                    }
                    else if (condition is Dictionary<string, string> conditionDict)
                    {
                        jsonPathCondition = new StubJsonPathModel
                        {
                            Query = conditionDict.ContainsKey("query")
                                ? conditionDict["query"]
                                : throw new InvalidOperationException($"Value 'query' not set for JSONPath condition for stub with ID '{stubId}'."),
                            ExpectedValue = conditionDict.ContainsKey("expectedValue")
                                ? conditionDict["expectedValue"]
                                : string.Empty
                        };
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Can't determine the type of the JSONPath condition for stub with ID '{stubId}'.");
                    }
                }
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            result.ConditionValidation = validJsonPaths == jsonPathConditions.Length
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;

            return result;
        }

        public int Priority => 0;
    }
}
