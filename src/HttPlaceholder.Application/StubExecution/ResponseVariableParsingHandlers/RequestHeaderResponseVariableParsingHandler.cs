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
///     Response variable parsing handler that is used to insert a request header in the response.
/// </summary>
internal class RequestHeaderResponseVariableParsingHandler(IHttpContextService httpContextService)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "request_header";

    /// <inheritdoc />
    public override string FullName => ResponseVariableParsingResources.RequestHeader;

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}:X-Api-Key))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.RequestHeaderDescription;

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken) =>
        matches
            .Where(match => match.Groups.Count >= 3)
            .Aggregate(input, (current, match) => InsertHeader(current, match, httpContextService.GetHeaders()))
            .AsTask();

    private static string InsertHeader(string current, Match match, IDictionary<string, string> headers)
    {
        var headerName = match.Groups[2].Value;
        var replaceValue = headers.CaseInsensitiveSearch(headerName);

        return current.Replace(match.Value, replaceValue);
    }
}
