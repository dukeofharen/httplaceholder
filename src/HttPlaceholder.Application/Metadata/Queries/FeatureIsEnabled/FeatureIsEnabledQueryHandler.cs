using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.Metadata.Queries.FeatureIsEnabled;

/// <summary>
/// A query handler that is used to check whether a specific feature is enabled or not.
/// </summary>
public class FeatureIsEnabledQueryHandler : IRequestHandler<FeatureIsEnabledQuery, FeatureResultModel>
{
    private readonly SettingsModel _settings;

    /// <summary>
    /// Constructs a <see cref="FeatureIsEnabledQueryHandler"/> instance.
    /// </summary>
    public FeatureIsEnabledQueryHandler(IOptions<SettingsModel> options)
    {
        _settings = options.Value;
    }

    /// <inheritdoc />
    public Task<FeatureResultModel> Handle(FeatureIsEnabledQuery request, CancellationToken cancellationToken) =>
        request.FeatureFlag switch
        {
            FeatureFlagType.Authentication => Task.FromResult(new FeatureResultModel(request.FeatureFlag,
                _settings.Authentication != null && !string.IsNullOrWhiteSpace(_settings.Authentication.ApiUsername) &&
                !string.IsNullOrWhiteSpace(_settings.Authentication.ApiPassword))),
            _ => throw new NotImplementedException($"Feature flag '{request.FeatureFlag}' not supported.")
        };
}
