using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class VariableParser : IVariableParser
    {
        public static Regex VarRegex = new Regex(@"\(\(([a-zA-Z0-9_]*)\:? ?([^)]*)?\)\)", RegexOptions.Compiled);

        private readonly IEnumerable<IVariableHandler> _handlers;

        public VariableParser(IEnumerable<IVariableHandler> handlers)
        {
            _handlers = handlers;
        }

        public string Parse(string input)
        {
            var matches = VarRegex.Matches(input).Cast<Match>();
            foreach (var handler in _handlers)
            {
                var handlerMatches = matches
                    .Where(m => m.Groups.Count > 1 && string.Equals(m.Groups[1].Value, handler.Name, StringComparison.OrdinalIgnoreCase));
                input = handler.Parse(input, handlerMatches);
            }

            return input;
        }
    }
}
