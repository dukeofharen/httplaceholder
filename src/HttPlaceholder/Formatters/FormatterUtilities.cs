using Microsoft.AspNetCore.Mvc;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HttPlaceholder.Formatters
{
    public static class FormatterUtilities
    {
        // ReSharper disable once UnusedMethodReturnValue.Global
        public static MvcOptions AddYamlFormatting(this MvcOptions options)
        {
            options.RespectBrowserAcceptHeader = true;
            options.ReturnHttpNotAcceptable = true;
            options.InputFormatters.Add(new YamlInputFormatter(new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()));
            options.OutputFormatters.Add(new YamlOutputFormatter(new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()));
            options.FormatterMappings.SetMediaTypeMappingForFormat("yaml", MediaTypeHeaderValues.ApplicationYaml);
            return options;
        }
    }
}
