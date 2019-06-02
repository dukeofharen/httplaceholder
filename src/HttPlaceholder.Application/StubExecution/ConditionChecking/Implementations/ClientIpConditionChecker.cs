using System.Linq;
using System.Net;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Application.StubExecution.ConditionChecking;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using NetTools;

namespace HttPlaceholder.Application.Implementations.ConditionCheckers
{
    public class ClientIpConditionChecker : IConditionChecker
    {
        private readonly IClientIpResolver _clientIpResolver;

        public ClientIpConditionChecker(IClientIpResolver clientIpResolver)
        {
            _clientIpResolver = clientIpResolver;
        }

        public ConditionCheckResultModel Validate(string stubId, StubConditionsModel conditions)
        {
            var result = new ConditionCheckResultModel();
            string clientIpCondition = conditions?.ClientIp;
            if (clientIpCondition != null)
            {
                var clientIp = IPAddress.Parse(_clientIpResolver.GetClientIp());
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
