using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class RequestBodyVariableHandler : IVariableHandler
    {
        private readonly IHttpContextService _httpContextService;

        public RequestBodyVariableHandler(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public string Name => "request_body";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            if (matches.Any())
            {
                string body = _httpContextService.GetBody();
                foreach (var match in matches)
                {
                    if (match.Groups.Count >= 2)
                    {
                        input = input.Replace(match.Value, body);
                    }
                }
            }

            return input;
        }
    }
}
