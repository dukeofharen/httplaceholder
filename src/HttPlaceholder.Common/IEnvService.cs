using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HttPlaceholder.Common;

public interface IEnvService
{
    IDictionary<string, string> GetEnvironmentVariables();

    string GetEnvironmentVariable(string key);

    bool IsOs(OSPlatform platform);
}