using System.Collections.Generic;
using MediatR;

namespace HttPlaceholder.Application.ScheduledJobs.Queries.GetScheduledJobNames;

/// <summary>
///     A query for retrieving all scheduled job names.
/// </summary>
public class GetScheduledJobNamesQuery : IRequest<IEnumerable<string>>
{
}
