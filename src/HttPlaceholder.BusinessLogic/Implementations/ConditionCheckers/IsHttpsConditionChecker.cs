using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
    public class IsHttpsConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public IsHttpsConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var condition = conditions?.Url?.IsHttps;
            if (condition != null)
            {
                bool shouldBeHttps = condition.Value;
                bool isHttps = _httpContextService.IsHttps();
                if (isHttps == shouldBeHttps)
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