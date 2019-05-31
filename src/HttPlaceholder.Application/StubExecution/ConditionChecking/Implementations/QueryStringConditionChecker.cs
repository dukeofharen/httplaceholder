using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Implementations.ConditionCheckers
{
    public class QueryStringConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public QueryStringConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var queryStringConditions = conditions?.Url?.Query;
            if (queryStringConditions != null)
            {
                int validQueryStrings = 0;
                var queryString = _httpContextService.GetQueryStringDictionary();
                foreach (var condition in queryStringConditions)
                {
                    // Check whether the condition query is available in the actual query string.
                    if (queryString.TryGetValue(condition.Key, out string queryValue))
                    {
                        // Check whether the condition query value is available in the actual query string.
                        string value = condition.Value ?? string.Empty;
                        if (!StringHelper.IsRegexMatchOrSubstring(queryValue, value))
                        {
                            // If the check failed, it means the query string is incorrect and the condition should fail.
                            result.Log = $"Query string condition '{condition.Key}: {condition.Value}' failed.";
                            break;
                        }

                        validQueryStrings++;
                    }
                }

                // If the number of succeeded conditions is equal to the actual number of conditions,
                // the query string condition is passed and the stub ID is passed to the result.
                if (validQueryStrings == queryStringConditions.Count)
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
