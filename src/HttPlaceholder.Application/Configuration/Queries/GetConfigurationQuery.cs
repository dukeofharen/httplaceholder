using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Application.Configuration.Queries;

/// <summary>
///     A query for retrieving the configuration
/// </summary>
public class GetConfigurationQuery : IRequest<IEnumerable<ConfigurationModel>>
{
}

/// <summary>
///     A handler for retrieving the configuration.
/// </summary>
public class GetConfigurationQueryHandler : IRequestHandler<GetConfigurationQuery, IEnumerable<ConfigurationModel>>
{
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     Constructs a <see cref="GetConfigurationQueryHandler" /> instance.
    /// </summary>
    public GetConfigurationQueryHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <inheritdoc />
    public Task<IEnumerable<ConfigurationModel>> Handle(
        GetConfigurationQuery request,
        CancellationToken cancellationToken)
    {
        var configMetadata = ConfigKeys.GetConfigMetadata();
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
                Key = item.Key,
                Path = item.Path,
                ConfigKeyType = item.ConfigKeyType,
                Description = item.Description,
                Value = value
            });
        }

        return Task.FromResult(result.AsEnumerable());
    }
}
