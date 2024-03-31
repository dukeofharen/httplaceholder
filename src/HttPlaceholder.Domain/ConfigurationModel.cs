using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Domain;

/// <summary>
///     A class for storing the data of a configuration item.
/// </summary>
public class ConfigurationModel(string key, string path, string description, ConfigKeyType configKeyType, string value)
{
    /// <summary>
    ///     Gets or sets the configuration item key.
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    ///     Gets or sets the configuration item path.
    /// </summary>
    public string Path { get; } = path;

    /// <summary>
    ///     Gets or sets the configuration item description.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    ///     Gets or sets the type of the config key.
    /// </summary>
    public ConfigKeyType ConfigKeyType { get; } = configKeyType;

    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    public string Value { get; } = value;
}
