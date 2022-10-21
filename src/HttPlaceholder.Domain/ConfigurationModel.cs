﻿using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Domain;

/// <summary>
///     A class for storing the data of a configuration item.
/// </summary>
public class ConfigurationModel
{
    /// <summary>
    ///     Gets or sets the configuration item key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    ///     Gets or sets the configuration item path.
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    ///     Gets or sets the configuration item description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the type of the config key.
    /// </summary>
    public ConfigKeyType ConfigKeyType { get; set; }

    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    public string Value { get; set; }
}
