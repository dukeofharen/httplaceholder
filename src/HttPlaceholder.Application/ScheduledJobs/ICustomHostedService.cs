using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace HttPlaceholder.Application.ScheduledJobs;

/// <summary>
///     Describes a class that implements a hosted service.
/// </summary>
public interface ICustomHostedService : IHostedService
{
    /// <summary>
    ///     Gets the job schedule as cron.
    /// </summary>
    string Schedule { get; }

    /// <summary>
    ///     Gets the hosted service key.
    /// </summary>
    string Key { get; }

    /// <summary>
    ///     Gets the hosted service description.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Gets the next run date/time of the hosted service.
    /// </summary>
    DateTime NextRunDateTime { get; }

    /// <summary>
    ///     Executed the hosted service.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task ProcessAsync(CancellationToken cancellationToken);
}
