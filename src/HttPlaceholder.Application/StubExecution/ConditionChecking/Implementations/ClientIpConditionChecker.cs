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

        public ConditionCheckResultModel Validate(StubModel stub)
        {
            var result = new ConditionCheckResultModel();
            var clientIpCondition = stub.Conditions?.ClientIp;
            if (clientIpCondition == null)
            {
                return result;
            }

            var clientIp = IPAddress.Parse(_clientDataResolver.GetClientIp());
            var ranges = IPAddressRange.Parse(clientIpCondition).AsEnumerable();
            result.ConditionValidation = ranges
                .Any(i => i.Equals(clientIp))
                ? ConditionValidationType.Valid
                : ConditionValidationType.Invalid;

            return result;
        }

        public int Priority => 10;
    }
}
