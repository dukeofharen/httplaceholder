using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.Metadata.Queries.FeatureIsEnabled;

public class FeatureIsEnabledQueryHandler : IRequestHandler<FeatureIsEnabledQuery, FeatureResultModel>
{
    private readonly SettingsModel _settings;

    public FeatureIsEnabledQueryHandler(IOptions<SettingsModel> options)
    {
        _settings = options.Value;
    }

    public Task<FeatureResultModel> Handle(FeatureIsEnabledQuery request, CancellationToken cancellationToken)
    {
        switch (request.FeatureFlag)
        {
            case FeatureFlagType.Authentication:
                return Task.FromResult(new FeatureResultModel(request.FeatureFlag,
                    _settings.Authentication != null &&
                    !string.IsNullOrWhiteSpace(_settings.Authentication.ApiUsername) &&
                    !string.IsNullOrWhiteSpace(_settings.Authentication.ApiPassword)));
            default:
                throw new NotImplementedException($"Feature flag '{request.FeatureFlag}' not supported.");
        }
    }
}