using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

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

        public string FullName => "Variable handler for inserting request header";

        public string Example => "((request_header:X-Api-Key))";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var headers = _httpContextService.GetHeaders();
            foreach (var match in matches)
            {
                if (match.Groups.Count != 3)
                {
                    continue;
                }

                var headerName = match.Groups[2].Value;
                headers.TryGetValue(headerName, out var replaceValue);

                input = input.Replace(match.Value, replaceValue);
            }

            return input;
        }
    }
}
