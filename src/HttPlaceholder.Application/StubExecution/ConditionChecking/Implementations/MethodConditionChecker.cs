using System;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Implementations.ConditionCheckers
{
    public class MethodConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public MethodConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            string methodCondition = conditions?.Method;
            if (!string.IsNullOrEmpty(methodCondition))
            {
                string method = _httpContextService.Method;
                if (string.Equals(methodCondition, method, StringComparison.OrdinalIgnoreCase))
                {
                    // The path matches the provided regex. Add the stub ID to the resulting list.
                    result.ConditionValidation = ConditionValidationType.Valid;
                }
                else
                {
                    result.Log = $"Condition '{methodCondition}' did not pass for request.";
                    result.ConditionValidation = ConditionValidationType.Invalid;
                }
            }

            return result;
        }
    }
}
