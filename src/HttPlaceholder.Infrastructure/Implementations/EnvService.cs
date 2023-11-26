using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class EnvService : IEnvService, ISingletonService
{
    /// <inheritdoc />
    public IDictionary<string, string> GetEnvironmentVariables() =>
        Environment.GetEnvironmentVariables()
            .Cast<DictionaryEntry>()
            .ToDictionary(de => (string)de.Key, de => (string)de.Value);

    /// <inheritdoc />
    public string GetEnvironmentVariable(string key) => Environment.GetEnvironmentVariable(key);

    /// <inheritdoc />
    public bool IsOs(OSPlatform platform) => RuntimeInformation.IsOSPlatform(platform);

    /// <inheritdoc />
    public string GetAspNetCoreEnvironment() => GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

    /// <inheritdoc />
    public string GetRuntime() => RuntimeInformation.FrameworkDescription;
}
