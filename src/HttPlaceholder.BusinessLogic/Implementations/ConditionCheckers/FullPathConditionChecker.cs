using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using HttPlaceholder.Utilities;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
    public class FullPathConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public FullPathConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            string fullPathCondition = conditions?.Url?.FullPath;
            if (!string.IsNullOrEmpty(fullPathCondition))
            {
                string path = _httpContextService.FullPath;
                if (StringHelper.IsRegexMatchOrSubstring(path, fullPathCondition))
                {
                    // The path matches the provided regex. Add the stub ID to the resulting list.
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
                else
                {
                    result.Log = $"Condition '{fullPathCondition}' did not pass for request.";
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
            }

            return result;
        }
    }
}