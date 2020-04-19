using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class IsHttpsConditionChecker : IConditionChecker
    {
        private readonly IClientDataResolver _clientDataResolver;

        public IsHttpsConditionChecker(IClientDataResolver clientDataResolver)
        {
            _clientDataResolver = clientDataResolver;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var condition = conditions?.Url?.IsHttps;
            if (condition != null)
            {
                var shouldBeHttps = condition.Value;
                var isHttps = _clientDataResolver.IsHttps();
                result.ConditionValidation = isHttps == shouldBeHttps
                    ? ConditionValidationType.Valid
                    : ConditionValidationType.Invalid;
            }

            return result;
        }
    }
}
