using HttPlaceholder.Common.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Web.Shared.Formatters;

/// <summary>
///     A class for adding several formatters to .NET.
/// </summary>
public static class FormatterUtilities
{
    /// <summary>
    ///     Adds YAML formatting.
    /// </summary>
    /// <param name="options">The <see cref="MvcOptions" />.</param>
    public static MvcOptions AddYamlFormatting(this MvcOptions options)
    {
        options.InputFormatters.Add(new YamlInputFormatter(YamlUtilities.BuildDeserializer()));
        options.OutputFormatters.Add(new YamlOutputFormatter(YamlUtilities.BuildSerializer()));
        options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
        return options;
    }

    /// <summary>
    ///     Adds plain text formatting.
    /// </summary>
    /// <param name="options">The <see cref="MvcOptions" />.</param>
    public static MvcOptions AddPlainTextFormatting(this MvcOptions options)
    {
        options.InputFormatters.Add(new PlainTextInputFormatter());
        return options;
    }
}
