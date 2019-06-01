using Ducode.Essentials.Mvc.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class RequestHeaderVariableHandler : IVariableHandler
    {
        private readonly IHttpContextService _httpContextService;

        public RequestHeaderVariableHandler(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public string Name => "request_header";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var headers = _httpContextService.GetHeaders();
            foreach (var match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    string headerName = match.Groups[2].Value;
                    string replaceValue = string.Empty;
                    headers.TryGetValue(headerName, out replaceValue);

                    input = input.Replace(match.Value, replaceValue);
                }
            }

            return input;
        }
    }
}
