using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers
{
    public class UuidResponseVariableParsingHandler : IResponseVariableParsingHandler
    {
        public string Name => "uuid";

        public string FullName => "Variable handler for inserting UUID";

        public string Example => "((uuid))";

        public string Parse(string input, IEnumerable<Match> matches) =>
            (from match
                    in matches
                where match.Groups.Count == 3
                select new Regex(Regex.Escape(match.Value)))
            .Aggregate(
                input, (current, replaceRegex) => replaceRegex.Replace(current, Guid.NewGuid().ToString(), 1));
    }
}
