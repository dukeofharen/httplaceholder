using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations
{
    public class EnvService : IEnvService
    {
        public IDictionary<string, string> GetEnvironmentVariables() =>
            Environment.GetEnvironmentVariables()
                .Cast<DictionaryEntry>()
                .ToDictionary(de => (string)de.Key, de => (string)de.Value);

        public string GetEnvironmentVariable(string key) => Environment.GetEnvironmentVariable(key);

        public bool IsOs(OSPlatform platform) => RuntimeInformation.IsOSPlatform(platform);
    }
}
