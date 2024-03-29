﻿using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Domain;

/// <summary>
///     A class for storing whether a specific feature is enabled or not.
/// </summary>
public class FeatureResultModel
{
    /// <summary>
    ///     Constructs a <see cref="FeatureResultModel" /> instance.
    /// </summary>
    /// <param name="featureFlag">The feature flag.</param>
    /// <param name="enabled">Whether the feature is enabled or not.</param>
    public FeatureResultModel(FeatureFlagType featureFlag, bool enabled)
    {
        FeatureFlag = featureFlag;
        Enabled = enabled;
    }

    /// <summary>
    ///     Gets or sets the checked feature.
    /// </summary>
    public FeatureFlagType FeatureFlag { get; }

    /// <summary>
    ///     Gets or sets whether the feature is enabled or not.
    /// </summary>
    public bool Enabled { get; }
}
