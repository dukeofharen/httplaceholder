using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler for generating a random UUID and putting it in the response.
/// </summary>
internal class UuidResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    /// <inheritdoc />
    public string Name => "uuid";

    /// <inheritdoc />
    public string FullName => "UUID";

    /// <inheritdoc />
    public string[] Examples => new[] {$"(({Name}))"};

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches, StubModel stub) =>
        (from match
                in matches
            where match.Groups.Count == 3
            select new Regex(Regex.Escape(match.Value)))
        .Aggregate(
            input, (current, replaceRegex) => replaceRegex.Replace(current, Guid.NewGuid().ToString(), 1));
}
