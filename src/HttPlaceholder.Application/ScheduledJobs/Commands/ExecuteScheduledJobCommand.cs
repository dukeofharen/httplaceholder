using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.ScheduledJobs;
using MediatR;

namespace HttPlaceholder.Application.ScheduledJobs.Commands;

/// <summary>
///     A command for executing a scheduled job.
/// </summary>
public class ExecuteScheduledJobCommand(string jobName) : IRequest<JobExecutionResultModel>
{
    /// <summary>
    ///     The job name of the job to execute.
    /// </summary>
    public string JobName { get; } = jobName;
}

/// <summary>
///     A command handler for executing a scheduled job.
/// </summary>
public class ExecuteScheduledJobCommandHandler(IEnumerable<ICustomHostedService> hostedServices)
    : IRequestHandler<ExecuteScheduledJobCommand, JobExecutionResultModel>
{
    /// <inheritdoc />
    public async Task<JobExecutionResultModel> Handle(
        ExecuteScheduledJobCommand request,
        CancellationToken cancellationToken)
    {
        var job = hostedServices.FirstOrDefault(s =>
                string.Equals(request.JobName, s.Key, StringComparison.OrdinalIgnoreCase))
            .IfNull(() =>
                throw new NotFoundException(string.Format(ApplicationResources.HostedJobNotFound, request.JobName)));
        try
        {
            await job.ProcessAsync(cancellationToken);
            return new JobExecutionResultModel("OK", false);
        }
        catch (Exception ex)
        {
            return new JobExecutionResultModel(ex.ToString(), true);
        }
    }
}
