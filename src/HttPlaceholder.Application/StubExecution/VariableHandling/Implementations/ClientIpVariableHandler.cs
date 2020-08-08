using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class ClientIpVariableHandler : IVariableHandler
    {
        private readonly IClientDataResolver _clientDataResolver;

        public ClientIpVariableHandler(IClientDataResolver clientDataResolver)
        {
            _clientDataResolver = clientDataResolver;
        }

        public string Name => "client_ip";

        public string FullName => "Client IP variable handler";

        public string Example => "((client_ip))";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var enumerable = matches as Match[] ?? matches.ToArray();
            if (!enumerable.Any())
            {
                return input;
            }

            var ip = _clientDataResolver.GetClientIp();
            return enumerable
                .Where(match => match.Groups.Count >= 2)
                .Aggregate(input, (current, match) => current.Replace(match.Value, ip));
        }
    }
}
