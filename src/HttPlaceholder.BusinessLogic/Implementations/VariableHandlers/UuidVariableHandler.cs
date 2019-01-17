using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HttPlaceholder.BusinessLogic.Implementations.VariableHandlers
{
    public class UuidVariableHandler : IVariableHandler
    {
        public string Name => "uuid";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            foreach (var match in matches)
            {
                if (match.Groups.Count == 3)
                {
                    var replaceRegex = new Regex(Regex.Escape(match.Value));

                    // Only replace first occurrence.
                    input = replaceRegex.Replace(input, Guid.NewGuid().ToString(), 1);
                }
            }

            return input;
        }
    }
}
