using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler for generating a random UUID and putting it in the response.
/// </summary>
internal class UuidResponseVariableParsingHandler : BaseVariableParsingHandler
{
    public UuidResponseVariableParsingHandler(IFileService fileService) : base(fileService)
    {
    }

    /// <inheritdoc />
    public override string Name => "uuid";

    /// <inheritdoc />
    public override string FullName => "UUID";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))"};

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub) =>
        (from match
                in matches
            where match.Groups.Count == 3
            select new Regex(Regex.Escape(match.Value)))
        .Aggregate(
            input, (current, replaceRegex) => replaceRegex.Replace(current, Guid.NewGuid().ToString(), 1));
}
