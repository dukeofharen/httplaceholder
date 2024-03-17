using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler for generating a random UUID and putting it in the response.
/// </summary>
internal class UuidResponseVariableParsingHandler(IFileService fileService)
    : BaseVariableParsingHandler(fileService), ISingletonService
{
    /// <inheritdoc />
    public override string Name => "uuid";

    /// <inheritdoc />
    public override string FullName => "UUID";

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}))"];

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken) =>
        Task.FromResult(matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, Guid.NewGuid().ToString())));
}
