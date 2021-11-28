using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttPlaceholder.Formatters
{
    public static class FormatterUtilities
    {
        public static MvcOptions AddYamlFormatting(this MvcOptions options)
        {
            options.InputFormatters.Add(new YamlInputFormatter(new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()));
            options.OutputFormatters.Add(new YamlOutputFormatter(new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()));
            options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
            return options;
        }

        public static MvcOptions AddPlainTextFormatting(this MvcOptions options)
        {
            options.InputFormatters.Add(new PlainTextInputFormatter());
            return options;
        }
    }
}
