using System;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers
{
    public class MethodConditionChecker : IConditionChecker
    {
        private readonly IHttpContextService _httpContextService;

        public MethodConditionChecker(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var methodCondition = stub.Conditions?.Method;
            if (string.IsNullOrEmpty(methodCondition))
            {
                return result;
            }

            var method = _httpContextService.Method;
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

            return result;
        }

        public int Priority => 10;
    }
}
