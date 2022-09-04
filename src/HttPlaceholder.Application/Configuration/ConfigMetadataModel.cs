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
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the configuration item name.
    /// </summary>
    public string DisplayKey { get; set; }

    /// <summary>
    /// Gets or sets the configuration item description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets the configuration item example.
    /// </summary>
    public string Example { get; set; }

    /// <summary>
    /// Gets or sets the configuration item path.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Gets or sets the configuration item is a boolean.
    /// </summary>
    public bool? IsBoolValue { get; set; }

    /// <summary>
    /// Gets or sets the type of the config key.
    /// </summary>
    public ConfigKeyType ConfigKeyType { get; set; }
}
