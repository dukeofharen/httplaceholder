using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using MediatR;

namespace HttPlaceholder.Application.Metadata.Queries.FeatureIsEnabled
{
    public class FeatureIsEnabledQuery : IRequest<FeatureResultModel>
    {
        public FeatureIsEnabledQuery(FeatureFlagType featureFlag)
        {
            FeatureFlag = featureFlag;
        }

        public FeatureFlagType FeatureFlag { get; }
    }
}
