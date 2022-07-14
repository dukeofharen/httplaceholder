using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to query the posted JSON string based on a JSONPath expression. The result is put in the response.
/// </summary>
internal class JsonPathResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IHttpContextService _httpContextService;
    private readonly ILogger<JsonPathResponseVariableParsingHandler> _logger;

    public JsonPathResponseVariableParsingHandler(
        IHttpContextService httpContextService,
        ILogger<JsonPathResponseVariableParsingHandler> logger)
    {
        _httpContextService = httpContextService;
        _logger = logger;
    }

    /// <inheritdoc />
    public string Name => "jsonpath";

    /// <inheritdoc />
    public string FullName => "JSONPath";

    /// <inheritdoc />
    public string Example => "((jsonpath:$.values[1].title))";

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var matchArray = matches as Match[] ?? matches.ToArray();
        if (!matchArray.Any())
        {
            return input;
        }

        var body = _httpContextService.GetBody();
        JObject jsonObject = null;
        try
        {
            jsonObject = JObject.Parse(body);
        }
        catch (JsonException je)
        {
            _logger.LogInformation($"Exception occurred while trying to parse response body as JSON: {je}");
        }

        return matchArray
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input,
                (current, match) => current.Replace(match.Value, GetJsonPathValue(match, jsonObject)));
    }

    private static string GetJsonPathValue(Match match, JToken token)
    {
        var jsonPathQuery = match.Groups[2].Value;
        var foundValue = token?.SelectToken(jsonPathQuery);
        return foundValue != null ? JsonUtilities.ConvertFoundValue(foundValue) : string.Empty;
    }
}
