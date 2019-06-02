using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Implementations.ConditionCheckers
{
    public class HostConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public HostConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var hostCondition = conditions?.Host;
            if (hostCondition != null)
            {
                string host = _httpContextService.GetHost();
                if (!StringHelper.IsRegexMatchOrSubstring(host, hostCondition))
                {
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
                else
                {
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
            }

            return result;
        }
    }
}
