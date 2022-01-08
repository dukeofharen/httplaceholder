﻿using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert a request header in the response.
/// </summary>
public class RequestHeaderResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;

    public RequestHeaderResponseVariableParsingHandler(IHttpContextService httpContextService)
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
            var headerName = match.Groups[2].Value;
            var replaceValue = headers.CaseInsensitiveSearch(headerName);

            input = input.Replace(match.Value, replaceValue);
        }

        return input;
    }
}
