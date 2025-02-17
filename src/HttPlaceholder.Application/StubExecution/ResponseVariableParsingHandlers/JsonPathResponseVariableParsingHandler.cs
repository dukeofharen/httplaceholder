﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to query the posted JSON string based on a JSONPath expression. The
///     result is put in the response.
/// </summary>
internal class JsonPathResponseVariableParsingHandler(
    IHttpContextService httpContextService,
    ILogger<JsonPathResponseVariableParsingHandler> logger)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "jsonpath";

    /// <inheritdoc />
    public override string FullName => ResponseVariableParsingResources.JsonPath;

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}:$.values[1].title))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.JsonPathDescription;

    /// <inheritdoc />
    protected override async Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var json = ParseJson(await httpContextService.GetBodyAsync(cancellationToken));
        return matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input,
                (current, match) => current.Replace(match.Value, GetJsonPathValue(match, json)));
    }

    private JObject ParseJson(string body)
    {
        try
        {
            return JObject.Parse(body);
        }
        catch (JsonException je)
        {
            logger.LogInformation(je, "Exception occurred while trying to parse response body as JSON.");
        }

        return null;
    }

    private static string GetJsonPathValue(Match match, JToken token)
    {
        var jsonPathQuery = match.Groups[2].Value;
        var foundValue = token?.SelectToken(jsonPathQuery);
        return foundValue != null ? JsonUtilities.ConvertFoundValue(foundValue) : string.Empty;
    }
}
