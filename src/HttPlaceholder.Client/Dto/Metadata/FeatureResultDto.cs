﻿using HttPlaceholder.Client.Dto.Enums;

namespace HttPlaceholder.Client.Dto.Metadata;

/// <summary>
///     A class for storing whether a specific feature is enabled or not.
/// </summary>
public class FeatureResultDto
{
    /// <summary>
    ///     Gets or sets the checked feature.
    /// </summary>
    public FeatureFlagType FeatureFlag { get; set; }

    /// <summary>
    ///     Gets or sets whether the feature is enabled or not.
    /// </summary>
    public bool Enabled { get; set; }
}
