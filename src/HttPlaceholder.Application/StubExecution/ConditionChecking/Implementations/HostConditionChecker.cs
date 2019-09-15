using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class HostConditionChecker : IConditionChecker
    {
        private readonly IClientDataResolver _clientDataResolver;

        public HostConditionChecker(IClientDataResolver clientDataResolver)
        {
            _clientDataResolver = clientDataResolver;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var hostCondition = conditions?.Host;
            if (hostCondition != null)
            {
                string host = _clientDataResolver.GetHost();
                result.ConditionValidation = !StringHelper.IsRegexMatchOrSubstring(host, hostCondition)
                    ? ConditionValidationType.Invalid
                    : ConditionValidationType.Valid;
            }

            return result;
        }
    }
}
