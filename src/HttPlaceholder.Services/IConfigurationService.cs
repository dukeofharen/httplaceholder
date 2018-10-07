using System.Collections.Generic;

namespace HttPlaceholder.Services
{
    public interface IConfigurationService
    {
        IDictionary<string, string> GetConfiguration();
    }
}