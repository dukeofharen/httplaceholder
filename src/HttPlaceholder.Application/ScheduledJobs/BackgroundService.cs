﻿using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace HttPlaceholder.Application.ScheduledJobs;

/// <summary>
///     An abstract class that needs to be implemented by any hosted service.
///     Source:
///     https://github.com/pgroene/ASPNETCoreScheduler/blob/master/ASPNETCoreScheduler/BackgroundService/BackgroundService.cs
/// </summary>
public abstract class BackgroundService : ICustomHostedService
{
    private readonly IAsyncService _asyncService;

    private readonly IDateTime _dateTime;

    /// <summary>
    ///     The logger.
    /// </summary>
    protected readonly ILogger<BackgroundService> Logger;

    internal readonly CancellationTokenSource StoppingCts = new();
    private CrontabSchedule _schedule;
    internal Task ExecutingTask;

    /// <summary>
    ///     Constructs a <see cref="BackgroundService" /> instance.
    /// </summary>
    protected BackgroundService(
        ILogger<BackgroundService> logger,
        IDateTime dateTime,
        IAsyncService asyncService)
    {
        Logger = logger;
        _dateTime = dateTime;
        _asyncService = asyncService;
        Initialize();
    }

    /// <inheritdoc />
    public abstract string Schedule { get; }

    /// <inheritdoc />
    public abstract string Key { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public DateTime NextRunDateTime { get; private set; }

    /// <inheritdoc />
    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        // Store the task we're executing.
        ExecutingTask = ExecuteAsync(StoppingCts.Token);

        // If the task is completed then return it, this will bubble cancellation and failure to the caller.
        return ExecutingTask.IsCompleted ? ExecutingTask : Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop called without start.
        if (ExecutingTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method.
            await StoppingCts.CancelAsync();
        }
        finally
        {
            // Wait until the task completes or the stop token triggers.
            await Task.WhenAny(ExecutingTask, Task.Delay(Timeout.Infinite,
                cancellationToken));
        }
    }

    /// <inheritdoc />
    public abstract Task ProcessAsync(CancellationToken cancellationToken);

    private void Initialize()
    {
        _schedule = CrontabSchedule.Parse(Schedule);
        NextRunDateTime = _schedule.GetNextOccurrence(_dateTime.Now);
        Logger.LogDebug(
            "New hosted service with name '{Service}' and schedule '{Schedule}' and the next occurrence will be on '{NextRunDateTime}'",
            GetType().Name, Schedule, NextRunDateTime);
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            if (_dateTime.Now > NextRunDateTime)
            {
                Logger.LogDebug(
                    "Executing hosted service with name '{Service}' and schedule '{Schedule}'", GetType().Name,
                    Schedule);
                try
                {
                    await ProcessAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Unexpected exception thrown while executing service '{Service}'.",
                        GetType().Name);
                }

                NextRunDateTime = _schedule.GetNextOccurrence(_dateTime.Now);
                Logger.LogInformation("Next run time of hosted service '{Service}': {NextRunDateTime}", GetType().Name,
                    NextRunDateTime);
            }

            await _asyncService.DelayAsync(5000, stoppingToken);
        } while (!stoppingToken.IsCancellationRequested);
    }
}
