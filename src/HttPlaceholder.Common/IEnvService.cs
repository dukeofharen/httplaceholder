using System.Collections.Generic;

namespace HttPlaceholder.Common
{
    public interface IEnvService
    {
        IDictionary<string, string> GetEnvironmentVariables();
    }
}
