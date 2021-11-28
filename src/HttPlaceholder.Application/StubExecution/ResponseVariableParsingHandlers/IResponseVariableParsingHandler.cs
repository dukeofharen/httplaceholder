using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers
{
    public interface IResponseVariableParsingHandler
    {
        string Name { get; }

        string FullName { get; }

        string Example { get; }

        string Parse(string input, IEnumerable<Match> matches);
    }
}
