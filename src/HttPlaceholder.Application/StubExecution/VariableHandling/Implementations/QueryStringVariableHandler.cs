﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class QueryStringVariableHandler : IVariableHandler
    {
        private readonly IHttpContextService _httpContextService;

        public QueryStringVariableHandler(IHttpContextService httpContextService)
        {
            _httpContextService = httpContextService;
        }

        public string Name => "query";

        public string FullName => "Query string variable handler";

        public string Example => "((query:query_string_key))";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var queryDict = _httpContextService.GetQueryStringDictionary();
            foreach (var match in matches)
            {
                if (match.Groups.Count != 3)
                {
                    continue;
                }

                var queryStringName = match.Groups[2].Value;
                queryDict.TryGetValue(queryStringName, out var replaceValue);

                input = input.Replace(match.Value, replaceValue);
            }

            return input;
        }
    }
}
