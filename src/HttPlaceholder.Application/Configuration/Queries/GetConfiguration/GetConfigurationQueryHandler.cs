using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Configuration.Queries.GetConfiguration;

/// <summary>
/// A handler for retrieving the configuration.
/// </summary>
public class GetConfigurationQueryHandler : IRequestHandler<GetConfigurationQuery, IEnumerable<ConfigurationModel>>
{
    /// <inheritdoc />
    public Task<IEnumerable<ConfigurationModel>> Handle(GetConfigurationQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
