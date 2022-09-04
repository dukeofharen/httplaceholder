using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Configuration;
using HttPlaceholder.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Application.Configuration.Queries.GetConfiguration;

/// <summary>
/// A handler for retrieving the configuration.
/// </summary>
public class GetConfigurationQueryHandler : IRequestHandler<GetConfigurationQuery, IEnumerable<ConfigurationModel>>
{
    private readonly IConfiguration _configuration;
    private readonly IConfigurationHelper _configurationHelper;

    /// <summary>
    /// Constructs a <see cref="GetConfigurationQueryHandler"/> instance.
    /// </summary>
    public GetConfigurationQueryHandler(IConfiguration configuration, IConfigurationHelper configurationHelper)
    {
        _configuration = configuration;
        _configurationHelper = configurationHelper;
    }

    /// <inheritdoc />
    public Task<IEnumerable<ConfigurationModel>> Handle(
        GetConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var configMetadata = _configurationHelper.GetConfigKeyMetadata();
        var result = new List<ConfigurationModel>();
        var configItems = _configuration.AsEnumerable().ToArray();
        foreach (var item in configMetadata)
        {
            var configItem =
                configItems.FirstOrDefault(i => string.Equals(item.Path, i.Key, StringComparison.OrdinalIgnoreCase));
            if (configItem.Value == null)
            {
                continue;
            }

            var value = item.IsSecretValue == true ? "***" : configItem.Value;
            result.Add(new ConfigurationModel
            {
                Key = item.Key, Path = item.Path, ConfigKeyType = item.ConfigKeyType, Value = value
            });
        }

        return Task.FromResult(result.AsEnumerable());
    }
}
