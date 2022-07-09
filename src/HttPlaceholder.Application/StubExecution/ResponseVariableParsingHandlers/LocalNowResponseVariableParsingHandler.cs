using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler to insert the local date/time into the response. An optional date/time format can be provided (based on the .NET date/time formatting strings).
/// </summary>
internal class LocalNowResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IDateTime _dateTime;

    public LocalNowResponseVariableParsingHandler(IDateTime dateTime)
    {
        _dateTime = dateTime;
    }

    /// <inheritdoc />
    public string Name => "localnow";

    /// <inheritdoc />
    public string FullName => "Local date / time";

    /// <inheritdoc />
    public string Example => "((localnow:yyyy-MM-dd HH:mm:ss))";

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var enumerable = matches as Match[] ?? matches.ToArray();
        if (!enumerable.Any())
        {
            return input;
        }

        var now = _dateTime.Now;
        foreach (var match in enumerable)
        {
            var dateTime = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                ? now.ToString(match.Groups[2].Value)
                : now.ToString(CultureInfo.InvariantCulture);
            input = input.Replace(match.Value, dateTime);
        }

        return input;
    }
}
