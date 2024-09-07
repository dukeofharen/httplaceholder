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
///     Response variable parsing handler that is used to insert the root URL (so the URL without path + query string) in
///     the response.
/// </summary>
internal class RootUrlResponseVariableParsingHandler(IUrlResolver urlResolver)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "root_url";

    /// <inheritdoc />
    public override string FullName => ResponseVariableParsingResources.RootUrl;

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.RootUrlDescription;

    protected override Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken) =>
        matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, urlResolver.GetRootUrl())).AsTask();
}
