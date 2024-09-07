using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.Metadata.Queries;

/// <summary>
///     A query that is used to check whether a specific feature is enabled or not.
/// </summary>
public class FeatureIsEnabledQuery : IRequest<FeatureResultModel>
{
    /// <summary>
    ///     Constructs a <see cref="FeatureIsEnabledQuery" /> instance.
    /// </summary>
    /// <param name="featureFlag">The <see cref="FeatureFlagType" /> to check for.</param>
    public FeatureIsEnabledQuery(FeatureFlagType featureFlag)
    {
        FeatureFlag = featureFlag;
    }

    /// <summary>
    ///     Gets the <see cref="FeatureFlagType" /> to check for.
    /// </summary>
    public FeatureFlagType FeatureFlag { get; }
}

/// <summary>
///     A query handler that is used to check whether a specific feature is enabled or not.
/// </summary>
public class FeatureIsEnabledQueryHandler(IOptionsMonitor<SettingsModel> options)
    : IRequestHandler<FeatureIsEnabledQuery, FeatureResultModel>
{
    /// <inheritdoc />
    public Task<FeatureResultModel> Handle(FeatureIsEnabledQuery request, CancellationToken cancellationToken)
    {
        var settings = options.CurrentValue;
        return request.FeatureFlag switch
        {
            FeatureFlagType.Authentication => new FeatureResultModel(request.FeatureFlag,
                settings.Authentication != null && !string.IsNullOrWhiteSpace(settings.Authentication.ApiUsername) &&
                !string.IsNullOrWhiteSpace(settings.Authentication.ApiPassword)).AsTask(),
            _ => throw new NotImplementedException(string.Format(ApplicationResources.FeatureFlagNotSupported,
                request.FeatureFlag))
        };
    }
}
