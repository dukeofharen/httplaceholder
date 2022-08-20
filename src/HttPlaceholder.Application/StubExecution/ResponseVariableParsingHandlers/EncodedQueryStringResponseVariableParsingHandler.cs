using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert a given URL encoded query parameter in the response.
/// </summary>
internal class EncodedQueryStringResponseVariableParsingHandler : BaseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public EncodedQueryStringResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService) : base(fileService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "query_encoded";

    /// <inheritdoc />
    public override string FullName => "URL encoded query string";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}:query_string_key))"};

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var queryDict = _httpContextService.GetQueryStringDictionary();
        foreach (var match in matches)
        {
            var queryStringName = match.Groups[2].Value;
            queryDict.TryGetValue(queryStringName, out var replaceValue);

            replaceValue = WebUtility.UrlEncode(replaceValue);
            input = input.Replace(match.Value, replaceValue);
        }

        return input;
    }
}
