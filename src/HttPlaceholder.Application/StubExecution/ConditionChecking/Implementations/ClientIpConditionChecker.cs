using System.Linq;
using System.Net;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using NetTools;

namespace HttPlaceholder.Application.StubExecution.ConditionChecking.Implementations
{
    public class ClientIpConditionChecker : IConditionChecker
    {
        private readonly IClientDataResolver _clientDataResolver;

        public ClientIpConditionChecker(IClientDataResolver clientDataResolver)
        {
            _clientDataResolver = clientDataResolver;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            var clientIpCondition = conditions?.ClientIp;
            if (clientIpCondition != null)
            {
                var clientIp = IPAddress.Parse(_clientDataResolver.GetClientIp());
                var ranges = IPAddressRange.Parse(clientIpCondition).AsEnumerable();
                if (ranges.Any(i => i.Equals(clientIp)))
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
