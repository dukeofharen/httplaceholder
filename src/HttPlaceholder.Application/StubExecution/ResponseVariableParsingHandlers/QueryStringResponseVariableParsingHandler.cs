using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert a given query parameter in the response.
/// </summary>
internal class QueryStringResponseVariableParsingHandler(IHttpContextService httpContextService)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "query";

    /// <inheritdoc />
    public override string FullName => "Query string";

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}:query_string_key))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.QueryDescription;

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken) =>
        matches
            .Where(match => match.Groups.Count >= 3)
            .Aggregate(input,
                (current, match) => InsertQuery(current, match, httpContextService.GetQueryStringDictionary()))
            .AsTask();

    private static string InsertQuery(string current, Match match, IDictionary<string, string> queryDict)
    {
        var queryStringName = match.Groups[2].Value;
        if (!queryDict.TryGetValue(queryStringName, out var replaceValue))
        {
            replaceValue = string.Empty;
        }

        return current.Replace(match.Value, replaceValue);
    }
}
