using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.VariableHandling
{
    public interface IVariableHandler
    {
        string Name { get; }

        string FullName { get; }

        string Example { get; }

        string Parse(string input, IEnumerable<Match> matches);
    }
}
