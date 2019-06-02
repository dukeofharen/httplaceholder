using System.Collections.Generic;
using System.Linq;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Configuration.Attributes;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Configuration.Utilities
{
    public static class ConfigurationUtilities
    {
        public static IEnumerable<(string configKey, string description, string example, string configPath)> GetConfigKeyMetadata()
        {
            var result = new List<(string, string, string, string)>();
            foreach (var constant in ReflectionUtilities.GetConstants(typeof(ConfigKeys)))
            {
                var attribute = constant.CustomAttributes.FirstOrDefault();
                if (attribute != null && attribute.AttributeType == typeof(ConfigKeyAttribute))
                {
                    result.Add((
                        constant.GetValue(constant) as string,
                        attribute.NamedArguments.Single(a => a.MemberName == "Description").TypedValue.Value as string,
                        attribute.NamedArguments.Single(a => a.MemberName == "Example").TypedValue.Value as string,
                        attribute.NamedArguments.FirstOrDefault(a => a.MemberName == "ConfigPath").TypedValue.Value as string));
                }
            }

            return result;
        }

        public static IConfigurationBuilder AddHttPlaceholderConfiguration(this IConfigurationBuilder builder, IDictionary<string, string> argsDictionary)
        {
            var configDictionary = new Dictionary<string, string>();
            foreach (var constant in GetConfigKeyMetadata())
            {
                if (constant.configPath != null && argsDictionary.TryGetValue(constant.configKey, out var value))
                {
                    configDictionary.Add(constant.configPath, value);
                }
            }

            return builder.AddInMemoryCollection(configDictionary);
        }
    }
}
