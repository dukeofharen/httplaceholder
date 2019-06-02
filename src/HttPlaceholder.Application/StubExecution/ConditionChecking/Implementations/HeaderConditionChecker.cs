using System.Linq;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Implementations.ConditionCheckers
{
    public class HeaderConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public HeaderConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var headerConditions = conditions?.Headers;
            if (headerConditions != null && headerConditions?.Any() == true)
            {
                int validHeaders = 0;
                var headers = _httpContextService.GetHeaders();
                foreach (var condition in headerConditions)
                {
                    // Check whether the condition header is available in the actual headers.
                    if (headers.TryGetValue(condition.Key, out string headerValue))
                    {
                        // Check whether the condition header value is available in the actual headers.
                        string value = condition.Value ?? string.Empty;
                        if (!StringHelper.IsRegexMatchOrSubstring(headerValue, value))
                        {
                            // If the check failed, it means the header is incorrect and the condition should fail.
                            result.Log = $"Header condition '{condition.Key}: {condition.Value}' failed.";
                            break;
                        }

                        validHeaders++;
                    }
                }

                // If the number of succeeded conditions is equal to the actual number of conditions,
                // the header condition is passed and the stub ID is passed to the result.
                if (validHeaders == headerConditions.Count)
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
