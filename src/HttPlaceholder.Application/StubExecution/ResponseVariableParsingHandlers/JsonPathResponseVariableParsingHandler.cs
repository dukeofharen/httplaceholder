using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
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
    ILogger<JsonPathResponseVariableParsingHandler> logger,
    IFileService fileService)
    : BaseVariableParsingHandler(fileService), ISingletonService
{
    /// <inheritdoc />
    public override string Name => "jsonpath";

    /// <inheritdoc />
    public override string FullName => "JSONPath";

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}:$.values[1].title))"];

    /// <inheritdoc />
    protected override async Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var body = await httpContextService.GetBodyAsync(cancellationToken);
        var json = ParseJson(body);
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
            logger.LogInformation($"Exception occurred while trying to parse response body as JSON: {je}");
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
