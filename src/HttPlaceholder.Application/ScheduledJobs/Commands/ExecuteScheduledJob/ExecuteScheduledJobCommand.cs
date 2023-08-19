using HttPlaceholder.Domain.ScheduledJobs;
using MediatR;

namespace HttPlaceholder.Application.ScheduledJobs.Commands.ExecuteScheduledJob;

/// <summary>
///     A command for executing a scheduled job.
/// </summary>
public class ExecuteScheduledJobCommand : IRequest<JobExecutionResultModel>
{
    /// <summary>
    ///     Constructs an <see cref="ExecuteScheduledJobCommand" /> instance.
    /// </summary>
    /// <param name="jobName"></param>
    public ExecuteScheduledJobCommand(string jobName)
    {
        JobName = jobName;
    }

    /// <summary>
    ///     The job name of the job to execute.
    /// </summary>
    public string JobName { get; private set; }
}
