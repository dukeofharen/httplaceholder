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

        public string FullName => "Display URL variable handler";

        public string Example => "((display_url))";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var enumerable = matches as Match[] ?? matches.ToArray();
            if (!enumerable.Any())
            {
                return input;
            }

            var url = _httpContextService.DisplayUrl;
            return enumerable
                .Where(match => match.Groups.Count >= 2)
                .Aggregate(input, (current, match) => current.Replace(match.Value, url));
        }
    }
}
