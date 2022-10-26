using Microsoft.Extensions.Configuration.Memory;

namespace HttPlaceholder.Application.Configuration.Provider;

/// <summary>
/// The HttPlaceholder configuration provider.
/// </summary>
public class HttPlaceholderConfigurationProvider : MemoryConfigurationProvider
{
    /// <summary>
    /// Constructs a <see cref="HttPlaceholderConfigurationProvider"/> instance.
    /// </summary>
    public HttPlaceholderConfigurationProvider(MemoryConfigurationSource source) : base(source)
    {
    }

    /// <summary>
    /// Reloads the configuration provider.
    /// </summary>
    public override void Load() => OnReload();
}
