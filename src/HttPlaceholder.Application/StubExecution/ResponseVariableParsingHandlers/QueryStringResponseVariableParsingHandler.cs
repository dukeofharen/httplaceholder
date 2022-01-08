using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert a given query parameter in the response.
/// </summary>
public class QueryStringResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public QueryStringResponseVariableParsingHandler(IHttpContextService httpContextService)
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
            var queryStringName = match.Groups[2].Value;
            queryDict.TryGetValue(queryStringName, out var replaceValue);

            input = input.Replace(match.Value, replaceValue);
        }

        return input;
    }
}
