using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.Configuration;

/// <summary>
/// A model used to contain the configuration key metadata.
/// </summary>
public class ConfigMetadataModel
{
    /// <summary>
    /// Gets or sets the configuration item key.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// Gets or sets the configuration item name.
    /// </summary>
    public string DisplayKey { get; private set; }

    /// <summary>
    /// Gets or sets the configuration item description.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets or sets the configuration item example.
    /// </summary>
    public string Example { get; private set; }

    /// <summary>
    /// Gets or sets the configuration item path.
    /// </summary>
    public string Path { get; private set; }

    /// <summary>
    /// Gets or sets the configuration item is a boolean.
    /// </summary>
    public bool? IsBoolValue { get; private set; }

    /// <summary>
    /// Gets or sets the type of the config key.
    /// </summary>
    public ConfigKeyType ConfigKeyType { get; private set; }

    /// <summary>
    /// Indicates whether the config value contains a secret.
    /// </summary>
    public bool? IsSecretValue { get; private set; }

    /// <summary>
    /// Creates a new <see cref="ConfigMetadataModel"/> instance
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="description">The description.</param>
    /// <param name="example">The example.</param>
    /// <param name="path">The path.</param>
    /// <param name="configKeyType">The config key type.</param>
    /// <param name="isBoolValue">Whether the value is a bool.</param>
    /// <param name="isSecretValue">Whether the value is a secret.</param>
    /// <returns>The <see cref="ConfigMetadataModel"/>.</returns>
    public static ConfigMetadataModel Create(
        string key,
        string description,
        string example,
        string path,
        ConfigKeyType configKeyType,
        bool? isBoolValue,
        bool? isSecretValue) =>
        new()
        {
            Key = key,
            Description = description,
            Example = example,
            Path = path,
            ConfigKeyType = configKeyType,
            DisplayKey = key.ToLower(),
            IsBoolValue = isBoolValue,
            IsSecretValue = isSecretValue
        };
}
