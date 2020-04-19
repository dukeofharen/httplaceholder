using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
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
            if (queryStringConditions == null || queryStringConditions?.Any() != true)
            {
                return result;
            }

            var validQueryStrings = 0;
            var queryString = _httpContextService.GetQueryStringDictionary();
            foreach (var condition in queryStringConditions)
            {
                // Check whether the condition query is available in the actual query string.
                if (!queryString.TryGetValue(condition.Key, out var queryValue))
                {
                    continue;
                }

                // Check whether the condition query value is available in the actual query string.
                var value = condition.Value ?? string.Empty;
                if (!StringHelper.IsRegexMatchOrSubstring(queryValue, value))
                {
                    // If the check failed, it means the query string is incorrect and the condition should fail.
                    result.Log = $"Query string condition '{condition.Key}: {condition.Value}' failed.";
                    break;
                }

                validQueryStrings++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the query string condition is passed and the stub ID is passed to the result.
            result.ConditionValidation = validQueryStrings == queryStringConditions.Count
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;

            return result;
        }
    }
}
