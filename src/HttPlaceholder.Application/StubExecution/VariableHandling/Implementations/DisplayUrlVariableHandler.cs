using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class DisplayUrlVariableHandler : IVariableHandler
    {
        private readonly IHttpContextService _httpContextService;

        public DisplayUrlVariableHandler(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public string Name => "display_url";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            if (matches.Any())
            {
                var url = _httpContextService.DisplayUrl;
                foreach (var match in matches)
                {
                    if (match.Groups.Count >= 2)
                    {
                        input = input.Replace(match.Value, url);
                    }
                }
            }

            return input;
        }
    }
}
