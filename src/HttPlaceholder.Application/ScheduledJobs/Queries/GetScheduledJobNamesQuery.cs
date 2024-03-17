using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace HttPlaceholder.Application.ScheduledJobs.Queries.GetScheduledJobNames;

/// <summary>
///     A query for retrieving all scheduled job names.
/// </summary>
public class GetScheduledJobNamesQuery : IRequest<IEnumerable<string>>
{
}

/// <summary>
///     A query handler for retrieving all scheduled job names.
/// </summary>
public class GetScheduledJobNamesQueryHandler(IEnumerable<ICustomHostedService> hostedServices)
    : IRequestHandler<GetScheduledJobNamesQuery, IEnumerable<string>>
{
    /// <inheritdoc />
    public Task<IEnumerable<string>> Handle(GetScheduledJobNamesQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(hostedServices.Select(s => s.Key));
}
