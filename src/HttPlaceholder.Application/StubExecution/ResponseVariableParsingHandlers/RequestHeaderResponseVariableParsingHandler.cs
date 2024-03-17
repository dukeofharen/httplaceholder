using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert a request header in the response.
/// </summary>
internal class RequestHeaderResponseVariableParsingHandler(
    IHttpContextService httpContextService,
    IFileService fileService)
    : BaseVariableParsingHandler(fileService), ISingletonService
{
    /// <inheritdoc />
    public override string Name => "request_header";

    /// <inheritdoc />
    public override string FullName => "Request header";

    /// <inheritdoc />
    public override string[] Examples => new[] { $"(({Name}:X-Api-Key))" };

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var headers = httpContextService.GetHeaders();
        return Task.FromResult(matches
            .Where(match => match.Groups.Count >= 3)
            .Aggregate(input, (current, match) => InsertHeader(current, match, headers)));
    }

    private static string InsertHeader(string current, Match match, IDictionary<string, string> headers)
    {
        var headerName = match.Groups[2].Value;
        var replaceValue = headers.CaseInsensitiveSearch(headerName);

        return current.Replace(match.Value, replaceValue);
    }
}
