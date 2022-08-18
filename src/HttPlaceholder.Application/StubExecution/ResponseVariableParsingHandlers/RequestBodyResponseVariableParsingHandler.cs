using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert the posted request body in the response.
/// </summary>
internal class RequestBodyResponseVariableParsingHandler : BaseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public RequestBodyResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService) : base(fileService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "request_body";

    /// <inheritdoc />
    public override string FullName => "Full request body";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))"};

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub)
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
