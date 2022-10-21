using HttPlaceholder.Application.Interfaces.Mappings;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Dto.v1.Metadata;

/// <summary>
///     A class for storing whether a specific feature is enabled or not.
/// </summary>
public class FeatureResultDto : IMapFrom<FeatureResultModel>
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
