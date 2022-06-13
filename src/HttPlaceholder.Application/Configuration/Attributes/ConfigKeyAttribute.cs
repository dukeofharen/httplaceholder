using System;

namespace HttPlaceholder.Application.Configuration.Attributes;

/// <summary>
/// An attribute that is used to decorate a config constant.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ConfigKeyAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets an example.
    /// </summary>
    public string Example { get; set; }

    /// <summary>
    /// Gets or sets the config path.
    /// </summary>
    public string ConfigPath { get; set; }

    /// <summary>
    /// Indicates whether the config value is a boolean.
    /// </summary>
    public bool IsBoolValue { get; set; }

    /// <summary>
    /// Gets or sets the type of the config key.
    /// </summary>
    public ConfigKeyType ConfigKeyType { get; set; }
}
