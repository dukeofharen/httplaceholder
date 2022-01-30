using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttPlaceholder.Formatters;

/// <summary>
/// A class for adding several formatters to .NET.
/// </summary>
public static class FormatterUtilities
{
    /// <summary>
    /// Adds YAML formatting.
    /// </summary>
    /// <param name="options">The <see cref="MvcOptions"/>.</param>
    public static MvcOptions AddYamlFormatting(this MvcOptions options)
    {
        options.InputFormatters.Add(new YamlInputFormatter(new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()));
        options.OutputFormatters.Add(new YamlOutputFormatter(new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()));
        options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
        return options;
    }

    /// <summary>
    /// Adds plain text formatting.
    /// </summary>
    /// <param name="options">The <see cref="MvcOptions"/>.</param>
    public static MvcOptions AddPlainTextFormatting(this MvcOptions options)
    {
        options.InputFormatters.Add(new PlainTextInputFormatter());
        return options;
    }
}
