using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class ResponseVariableParser : IResponseVariableParser, ISingletonService
{
    private readonly IEnumerable<IResponseVariableParsingHandler> _handlers;

    public ResponseVariableParser(IEnumerable<IResponseVariableParsingHandler> handlers)
    {
        _handlers = handlers;
    }

    public static Regex VarRegex { get; } = new(
        @"\(\(([a-zA-Z0-9_]*)\:? ?([^)]*)?\)\)",
        RegexOptions.Compiled,
        TimeSpan.FromSeconds(10));

    /// <inheritdoc />
    public async Task<string> ParseAsync(string input, StubModel stub, CancellationToken cancellationToken)
    {
        var matches = VarRegex.Matches(input).ToArray();
        foreach (var handler in _handlers)
        {
            var handlerMatches = matches
                .Where(m => m.Groups.Count > 1 && string.Equals(m.Groups[1].Value, handler.Name,
                    StringComparison.OrdinalIgnoreCase));
            input = await handler.ParseAsync(input, handlerMatches, stub, cancellationToken);
        }

        return input;
    }
}
