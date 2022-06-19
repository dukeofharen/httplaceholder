using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Attributes;
using HttPlaceholder.Common.Utilities;

namespace HttPlaceholder.Infrastructure.Configuration;

/// <summary>
/// A class that contains several configuration related methods.
/// </summary>
public static class ConfigurationHelper
{
    /// <summary>
    /// Returns a list of all possible configuration keys and its metadata.
    /// </summary>
    /// <returns>A list of <see cref="ConfigMetadataModel"/>.</returns>
    public static IList<ConfigMetadataModel> GetConfigKeyMetadata() =>
        (from constant
                in ReflectionUtilities.GetConstants(typeof(ConfigKeys))
            let attribute = constant.CustomAttributes.FirstOrDefault()
            where attribute != null && attribute.AttributeType == typeof(ConfigKeyAttribute)
            select new ConfigMetadataModel
            {
                Key = (constant.GetValue(constant) as string)?.ToLower(),
                DisplayKey = constant.GetValue(constant) as string,
                Description = ParseAttribute<string>(attribute, "Description", true),
                Example = ParseAttribute<string>(attribute, "Example", true),
                Path = ParseAttribute<string>(attribute, "ConfigPath", false),
                IsBoolValue = ParseAttribute<bool?>(attribute, "IsBoolValue", false),
                ConfigKeyType = ParseAttribute<ConfigKeyType>(attribute, "ConfigKeyType", true)
            }).ToList();

    private static TValue ParseAttribute<TValue>(CustomAttributeData attribute, string memberName, bool shouldExist)
    {
        if (attribute.NamedArguments == null)
        {
            throw new InvalidOperationException("NamedArguments not set.");
        }

        var result =
            attribute.NamedArguments.FirstOrDefault(a => a.MemberName == memberName).TypedValue.Value;
        if (shouldExist && result == null)
        {
            throw new InvalidOperationException(
                $"Property with name '{memberName}' was not found, but it should exist.");
        }

        return (TValue)result;
    }
}
