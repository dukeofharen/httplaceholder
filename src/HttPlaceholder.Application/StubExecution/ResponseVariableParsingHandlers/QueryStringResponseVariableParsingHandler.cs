using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert a given query parameter in the response.
/// </summary>
internal class QueryStringResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public QueryStringResponseVariableParsingHandler(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public string Name => "query";

    /// <inheritdoc />
    public string FullName => "Query string";

    /// <inheritdoc />
    public string Example => "((query:query_string_key))";

    /// <inheritdoc />
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
