using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Application.Configuration.Provider;

/// <summary>
///     A static class containing extension methods for the HttPlaceholder configuration source.
/// </summary>
public static class HttPlaceholderConfigurationExtensions
{
    /// <summary>
    ///     Adds the HttPlaceholder configuration source.
    /// </summary>
    /// <param name="builder">The configuration builder.</param>
    /// <param name="args">Initial data.</param>
    /// <returns>The configuration builder.</returns>
    public static IConfigurationBuilder AddCustomInMemoryCollection(this IConfigurationBuilder builder,
        IDictionary<string, string> args) => builder.Add(new HttPlaceholderConfigurationSource {InitialData = args});
}
