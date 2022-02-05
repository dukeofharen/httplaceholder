﻿using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace HttPlaceholder.HostedServices;

/// <summary>
/// An abstract class that needs to be implemented by any hosted service.
/// Source: https://github.com/pgroene/ASPNETCoreScheduler/blob/master/ASPNETCoreScheduler/BackgroundService/BackgroundService.cs
/// </summary>
public abstract class BackgroundService : ICustomHostedService
{
    private Task _executingTask;
    private readonly CancellationTokenSource _stoppingCts = new();
    private readonly CrontabSchedule _schedule;

    /// <summary>
    /// The logger.
    /// </summary>
    protected readonly ILogger<BackgroundService> Logger;

    private readonly IDateTime _dateTime;

    /// <summary>
    /// Constructs a <see cref="BackgroundService"/> instance.
    /// </summary>
    protected BackgroundService(
        ILogger<BackgroundService> logger,
        IDateTime dateTime)
    {
        _schedule = CrontabSchedule.Parse(Schedule);
        Logger = logger;
        _dateTime = dateTime;
        NextRunDateTime = _schedule.GetNextOccurrence(_dateTime.Now);
        Logger.LogInformation(
            $"New hosted service with name '{GetType().Name}' and schedule '{Schedule}' and the next occurrence will be on '{NextRunDateTime}'");
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
        _executingTask = ExecuteAsync(_stoppingCts.Token);

        // If the task is completed then return it, this will bubble cancellation and failure to the caller.
        return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
    }

    /// <inheritdoc />
    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop called without start
        if (_executingTask == null)
        {
            return;
        }

        try
        {
            // Signal cancellation to the executing method.
            _stoppingCts.Cancel();
        }
        finally
        {
            // Wait until the task completes or the stop token triggers.
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                cancellationToken));
        }
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        do
        {
            if (_dateTime.Now > NextRunDateTime)
            {
                Logger.LogInformation(
                    $"Executing hosted service with name '{GetType().Name}' and schedule '{Schedule}'");
                try
                {
                    await ProcessAsync();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Unexpected exception thrown while executing service '{GetType().Name}': {ex}");
                }

                NextRunDateTime = _schedule.GetNextOccurrence(_dateTime.Now);
                Logger.LogDebug($"Next run time of hosted service '{GetType().Name}': {NextRunDateTime}");
            }

            await Task.Delay(5000, stoppingToken);
        } while (!stoppingToken.IsCancellationRequested);
    }

    /// <inheritdoc />
    public abstract Task ProcessAsync();
}
