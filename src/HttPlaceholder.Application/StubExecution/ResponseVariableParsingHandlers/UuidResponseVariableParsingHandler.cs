using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler for generating a random UUID and putting it in the response.
/// </summary>
public class UuidResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    public string Name => "uuid";

    public string FullName => "UUID";

    public string Example => "((uuid))";

    public string Parse(string input, IEnumerable<Match> matches) =>
        (from match
                in matches
            where match.Groups.Count == 3
            select new Regex(Regex.Escape(match.Value)))
        .Aggregate(
            input, (current, replaceRegex) => replaceRegex.Replace(current, Guid.NewGuid().ToString(), 1));
}
