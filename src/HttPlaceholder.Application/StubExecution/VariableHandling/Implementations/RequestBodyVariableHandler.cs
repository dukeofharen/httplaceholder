﻿using System.Collections.Generic;
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

        public string FullName => "Variable handler for inserting complete request body";

        public string Example => "((request_body))";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var enumerable = matches as Match[] ?? matches.ToArray();
            if (!enumerable.Any())
            {
                return input;
            }

            var body = _httpContextService.GetBody();

            return enumerable
                .Where(match => match.Groups.Count >= 2)
                .Aggregate(input, (current, match) => current.Replace(match.Value, body));
        }
    }
}
