using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

namespace HttPlaceholder.Application.StubExecution.Implementations
{
    public class ResponseVariableParser : IResponseVariableParser
    {
        public static Regex VarRegex { get; } = new Regex(
            @"\(\(([a-zA-Z0-9_]*)\:? ?([^)]*)?\)\)",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(10));

        private readonly IEnumerable<IResponseVariableParsingHandler> _handlers;

        public ResponseVariableParser(IEnumerable<IResponseVariableParsingHandler> handlers)
        {
            _handlers = handlers;
        }

        public string Parse(string input)
        {
            var matches = VarRegex.Matches(input).Cast<Match>();
            foreach (var handler in _handlers)
            {
                var handlerMatches = matches
                    .Where(m => m.Groups.Count > 1 && string.Equals(m.Groups[1].Value, handler.Name,
                                    StringComparison.OrdinalIgnoreCase));
                input = handler.Parse(input, handlerMatches);
            }

            return input;
        }
    }
}
