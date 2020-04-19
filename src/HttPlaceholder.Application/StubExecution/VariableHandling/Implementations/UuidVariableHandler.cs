using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class UuidVariableHandler : IVariableHandler
    {
        public string Name => "uuid";

        public string Parse(string input, IEnumerable<Match> matches) =>
            (from match
                    in matches
                where match.Groups.Count == 3
                select new Regex(Regex.Escape(match.Value)))
            .Aggregate(
                input, (current, replaceRegex) => replaceRegex.Replace(current, Guid.NewGuid().ToString(), 1));
    }
}
