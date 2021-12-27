using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler to insert the UTC date/time into the response. An optional date/time format can be provided (based on the .NET date/time formatting strings).
/// </summary>
public class UtcNowResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IDateTime _dateTime;

    public UtcNowResponseVariableParsingHandler(IDateTime dateTime)
    {
        _dateTime = dateTime;
    }


    public string Name => "utcnow";

    public string FullName => "Variable handler for retrieving UTC date / time";

    public string Example => "((utcnow:yyyy-MM-dd HH:mm:ss))";

    public string Parse(string input, IEnumerable<Match> matches)
    {
        var enumerable = matches as Match[] ?? matches.ToArray();
        if (!enumerable.Any())
        {
            return input;
        }

        var now = _dateTime.UtcNow;
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
