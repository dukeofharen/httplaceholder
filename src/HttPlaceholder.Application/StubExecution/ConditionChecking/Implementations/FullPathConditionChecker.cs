using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class FullPathConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public FullPathConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var fullPathCondition = stub.Conditions?.Url?.FullPath;
            if (string.IsNullOrEmpty(fullPathCondition))
            {
                return result;
            }

            var path = _httpContextService.FullPath;
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

            return result;
        }

        public int Priority => 9;
    }
}
