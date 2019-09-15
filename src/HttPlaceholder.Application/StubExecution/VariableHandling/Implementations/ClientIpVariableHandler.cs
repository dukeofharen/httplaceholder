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

        public string Parse(string input, IEnumerable<Match> matches)
        {
            if (matches.Any())
            {
                var ip = _clientDataResolver.GetClientIp();
                foreach (var match in matches)
                {
                    if (match.Groups.Count >= 2)
                    {
                        input = input.Replace(match.Value, ip);
                    }
                }
            }

            return input;
        }
    }
}
