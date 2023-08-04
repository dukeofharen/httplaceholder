using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.Interfaces.HostedServices;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.ScheduledJobs;
using MediatR;

namespace HttPlaceholder.Application.ScheduledJobs.Commands.ExecuteScheduledJob;

/// <summary>
///     A command handler for executing a scheduled job.
/// </summary>
public class ExecuteScheduledJobCommandHandler : IRequestHandler<ExecuteScheduledJobCommand, JobExecutionResultModel>
{
    private readonly IEnumerable<ICustomHostedService> _hostedServices;

    /// <summary>
    ///     Constructs an <see cref="ExecuteScheduledJobCommandHandler"/> instance.
    /// </summary>
    public ExecuteScheduledJobCommandHandler(IEnumerable<ICustomHostedService> hostedServices)
    {
        _hostedServices = hostedServices;
    }

    /// <inheritdoc />
    public async Task<JobExecutionResultModel> Handle(ExecuteScheduledJobCommand request, CancellationToken cancellationToken)
    {
        var message = "OK";
        var job = _hostedServices.FirstOrDefault(s =>
                string.Equals(request.JobName, s.Key, StringComparison.OrdinalIgnoreCase))
            .IfNull(() => throw new NotFoundException($"Hosted service with key '{request.JobName}'."));
        bool failed;
        try
        {
            await job.ProcessAsync(cancellationToken);
            failed = false;
        }
        catch (Exception ex)
        {
            message = ex.ToString();
            failed = true;
        }

        return new JobExecutionResultModel(message, failed);
    }
}
