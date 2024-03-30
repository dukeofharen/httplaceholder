using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using MediatR;

namespace HttPlaceholder.Application.ScheduledJobs.Queries;

/// <summary>
///     A query for retrieving all scheduled job names.
/// </summary>
public class GetScheduledJobNamesQuery : IRequest<IEnumerable<string>>;

/// <summary>
///     A query handler for retrieving all scheduled job names.
/// </summary>
public class GetScheduledJobNamesQueryHandler(IEnumerable<ICustomHostedService> hostedServices)
    : IRequestHandler<GetScheduledJobNamesQuery, IEnumerable<string>>
{
    /// <inheritdoc />
    public Task<IEnumerable<string>> Handle(GetScheduledJobNamesQuery request, CancellationToken cancellationToken) =>
        hostedServices.Select(s => s.Key).AsTask();
}
