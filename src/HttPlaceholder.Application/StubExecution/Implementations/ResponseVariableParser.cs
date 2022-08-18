using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc/>
internal class ResponseVariableParser : IResponseVariableParser
{
    public static Regex VarRegex { get; } = new(
        @"\(\(([a-zA-Z0-9_]*)\:? ?([^)]*)?\)\)",
        RegexOptions.Compiled,
        TimeSpan.FromSeconds(10));

    private readonly IEnumerable<IResponseVariableParsingHandler> _handlers;

    public ResponseVariableParser(IEnumerable<IResponseVariableParsingHandler> handlers)
    {
        _handlers = handlers;
    }

    /// <inheritdoc/>
    public string Parse(string input, StubModel stub)
    {
        var matches = VarRegex.Matches(input).ToArray();
        foreach (var handler in _handlers)
        {
            // TODO make this a bit more efficient (e.g. directly filter for found placeholders).
            var handlerMatches = matches
                .Where(m => m.Groups.Count > 1 && string.Equals(m.Groups[1].Value, handler.Name,
                    StringComparison.OrdinalIgnoreCase));
            input = handler.Parse(input, handlerMatches, stub);
        }

        return input;
    }
}
