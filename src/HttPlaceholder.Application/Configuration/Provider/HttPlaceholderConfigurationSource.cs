using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace HttPlaceholder.Application.Configuration.Provider;

/// <summary>
///     The HttPlaceholder configuration source.
/// </summary>
public class HttPlaceholderConfigurationSource : IConfigurationSource
{
    /// <summary>
    ///     The initial key value configuration pairs.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> InitialData { get; set; }

    /// <inheritdoc />
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new HttPlaceholderConfigurationProvider(new MemoryConfigurationSource {InitialData = InitialData});
}
