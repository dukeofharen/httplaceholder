using System.Collections.Generic;

namespace HttPlaceholder.Services
{
    public interface IConfigurationParser
    {
        IDictionary<string, string> ParseConfiguration(string[] args);
    }
}
