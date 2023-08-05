using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace HttPlaceholder.Application.ScheduledJobs.Queries.GetScheduledJobNames;

/// <summary>
///     A query handler for retrieving all scheduled job names.
/// </summary>
public class GetScheduledJobNamesQueryHandler : IRequestHandler<GetScheduledJobNamesQuery, IEnumerable<string>>
{
    private readonly IEnumerable<ICustomHostedService> _hostedServices;

    /// <summary>
    ///     Constructs an <see cref="GetScheduledJobNamesQueryHandler"/> instance.
    /// </summary>
    public GetScheduledJobNamesQueryHandler(IEnumerable<ICustomHostedService> hostedServices)
    {
        _hostedServices = hostedServices;
    }

    /// <inheritdoc />
    public Task<IEnumerable<string>> Handle(GetScheduledJobNamesQuery request, CancellationToken cancellationToken) =>
        Task.FromResult(_hostedServices.Select(s => s.Key));
}
