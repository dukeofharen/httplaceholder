using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HttPlaceholder.Application.StubExecution.VariableHandling
{
    public interface IVariableHandler
    {
        string Name { get; }

        string Parse(string input, IEnumerable<Match> matches);
    }
}
