using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert the posted request body in the response.
/// </summary>
public class RequestBodyResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public RequestBodyResponseVariableParsingHandler(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    public string Name => "request_body";

    public string FullName => "Variable handler for inserting complete request body";

    public string Example => "((request_body))";

    public string Parse(string input, IEnumerable<Match> matches)
    {
        var matchArray = matches as Match[] ?? matches.ToArray();
        if (!matchArray.Any())
        {
            return input;
        }

        var body = _httpContextService.GetBody();

        return matchArray
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, body));
    }
}
