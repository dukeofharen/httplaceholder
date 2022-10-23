using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.HostedServices;

/// <summary>
///     A class that is used to handle several application events.
/// </summary>
public class LifetimeEventsHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly ILogger<LifetimeEventsHostedService> _logger;

    /// <summary>
    ///     Constructs a <see cref="LifetimeEventsHostedService"/> instance.
    /// </summary>
    public LifetimeEventsHostedService(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<LifetimeEventsHostedService> logger)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _hostApplicationLifetime.ApplicationStarted.Register(() => _logger.LogInformation($"Starting the application took {Program.GetStartupMillis()} ms."));
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
