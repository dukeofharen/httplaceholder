using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;

namespace HttPlaceholder.Application.StubExecution.VariableHandling.Implementations
{
    public class LocalNowVariableHandler : IVariableHandler
    {
        private readonly IDateTime _dateTime;

        public LocalNowVariableHandler(IDateTime dateTime)
        {
            _dateTime = dateTime;
        }


        public string Name => "localnow";

        public string Parse(string input, IEnumerable<Match> matches)
        {
            var enumerable = matches as Match[] ?? matches.ToArray();
            if (enumerable.Any())
            {
                var now = _dateTime.Now;
                foreach (var match in enumerable)
                {
                    var dateTime = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                        ? now.ToString(match.Groups[2].Value)
                        : now.ToString(CultureInfo.InvariantCulture);
                    input = input.Replace(match.Value, dateTime);
                }
            }

            return input;
        }
    }
}
