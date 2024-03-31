using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace HttPlaceholder.Application.Configuration.Queries;

/// <summary>
///     A query for retrieving the configuration
/// </summary>
public class GetConfigurationQuery : IRequest<IEnumerable<ConfigurationModel>>;

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
        CancellationToken cancellationToken) =>
        (from item in ConfigKeys.GetConfigMetadata()
            let configItem =
                _configuration.AsEnumerable()
                    .FirstOrDefault(i => string.Equals(item.Path, i.Key, StringComparison.OrdinalIgnoreCase))
            where configItem.Value != null
            select new ConfigurationModel(
                item.Key,
                item.Path,
                item.Description,
                item.ConfigKeyType,
                item.IsSecretValue == true ? "***" : configItem.Value)).AsTask();
}
