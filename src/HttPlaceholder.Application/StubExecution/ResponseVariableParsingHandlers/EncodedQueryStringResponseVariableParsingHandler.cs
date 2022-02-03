﻿using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert a given URL encoded query parameter in the response.
/// </summary>
internal class EncodedQueryStringResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public EncodedQueryStringResponseVariableParsingHandler(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public string Name => "query_encoded";

    /// <inheritdoc />
    public string FullName => "URL encoded query string";

    /// <inheritdoc />
    public string Example => "((query_encoded:query_string_key))";

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches)
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
