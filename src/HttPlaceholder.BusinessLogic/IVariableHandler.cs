using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HttPlaceholder.BusinessLogic
{
    public interface IVariableHandler
    {
        string Name { get; }

        string Parse(string input, IEnumerable<Match> matches);
    }
}
