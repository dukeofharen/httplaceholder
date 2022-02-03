using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using MediatR;

namespace HttPlaceholder.Application.Metadata.Queries.FeatureIsEnabled;

/// <summary>
/// A query that is used to check whether a specific feature is enabled or not.
/// </summary>
public class FeatureIsEnabledQuery : IRequest<FeatureResultModel>
{
    /// <summary>
    /// Constructs a <see cref="FeatureIsEnabledQuery"/> instance.
    /// </summary>
    /// <param name="featureFlag">The <see cref="FeatureFlagType"/> to check for.</param>
    public FeatureIsEnabledQuery(FeatureFlagType featureFlag)
    {
        FeatureFlag = featureFlag;
    }

    /// <summary>
    /// Gets the <see cref="FeatureFlagType"/> to check for.
    /// </summary>
    public FeatureFlagType FeatureFlag { get; }
}
