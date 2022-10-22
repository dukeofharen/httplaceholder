using System.Collections.Generic;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Configuration.Queries.GetConfiguration;

/// <summary>
///     A query for retrieving the configuration
/// </summary>
public class GetConfigurationQuery : IRequest<IEnumerable<ConfigurationModel>>
{
}
