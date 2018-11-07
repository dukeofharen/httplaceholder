using System.Linq;
using System.Net;
using Ducode.Essentials.Mvc.Interfaces;
using HttPlaceholder.Models;
using HttPlaceholder.Models.Enums;
using HttPlaceholder.Services;
using NetTools;

namespace HttPlaceholder.BusinessLogic.Implementations.ConditionCheckers
{
    public class ClientIpConditionChecker : IConditionChecker
    {
        private readonly IClientIpResolver _clientIpResolver;
        private readonly IHttpContextService _httpContextService;

        public ClientIpConditionChecker(
            IClientIpResolver clientIpResolver,
            IHttpContextService httpContextService)
        {
            _clientIpResolver = clientIpResolver;
            _httpContextService = httpContextService;
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