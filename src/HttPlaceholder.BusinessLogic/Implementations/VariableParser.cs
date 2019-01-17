using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Models;

namespace HttPlaceholder.BusinessLogic.Implementations
{
    public class VariableParser : IVariableParser
    {
        private readonly IEnumerable<IVariableHandler> _handlers;

        public VariableParser(IEnumerable<IVariableHandler> handlers)
        {
            _handlers = handlers;
        }

        public string Parse(string input)
        {
            var matches = Constants.Regexes.VarRegex.Matches(input).Cast<Match>();
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
