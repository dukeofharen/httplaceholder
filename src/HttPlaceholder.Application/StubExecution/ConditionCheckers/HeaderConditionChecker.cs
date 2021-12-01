using System.Linq;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers
{
    public class HeaderConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public HeaderConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var headerConditions = stub.Conditions?.Headers;
            if (headerConditions == null || headerConditions?.Any() != true)
            {
                return result;
            }

            var validHeaders = 0;
            var headers = _httpContextService.GetHeaders();
            foreach (var condition in headerConditions)
            {
                // Check whether the condition header is available in the actual headers.
                var headerValue = headers.CaseInsensitiveSearch(condition.Key);
                if (string.IsNullOrWhiteSpace(headerValue))
                {
                    continue;
                }

                // Check whether the condition header value is available in the actual headers.
                var value = condition.Value ?? string.Empty;
                if (!StringHelper.IsRegexMatchOrSubstring(headerValue, value))
                {
                    // If the check failed, it means the header is incorrect and the condition should fail.
                    result.Log = $"Header condition '{condition.Key}: {condition.Value}' failed.";
                    break;
                }

                validHeaders++;
            }

            // If the number of succeeded conditions is equal to the actual number of conditions,
            // the header condition is passed and the stub ID is passed to the result.
            result.ConditionValidation = validHeaders == headerConditions.Count
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;
            return result;
        }

        public int Priority => 8;
    }
}
