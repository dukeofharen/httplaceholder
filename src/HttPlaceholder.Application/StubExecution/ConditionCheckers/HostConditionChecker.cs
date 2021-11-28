using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers
{
    public class HostConditionChecker : IConditionChecker
    {
        private readonly IClientDataResolver _clientDataResolver;

        public HostConditionChecker(IClientDataResolver clientDataResolver)
        {
            _clientDataResolver = clientDataResolver;
        }

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var hostCondition = stub.Conditions?.Host;
            if (hostCondition == null)
            {
                return result;
            }

            var host = _clientDataResolver.GetHost();
            result.ConditionValidation = !StringHelper.IsRegexMatchOrSubstring(host, hostCondition)
                ? ConditionValidationType.Invalid
                : ConditionValidationType.Valid;

            return result;
        }

        public int Priority => 10;
    }
}
