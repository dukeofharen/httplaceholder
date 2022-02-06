﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Authorization;
using HttPlaceholder.Dto.v1.ScheduledJobs;
using HttPlaceholder.HostedServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttPlaceholder.Controllers.v1;

/// <summary>
/// A controller that is used for manually calling a scheduled job.
/// </summary>
[Route("ph-api/scheduledJob")]
[ApiAuthorization]
public class ScheduledJobController : BaseApiController
{
    private readonly IEnumerable<ICustomHostedService> _hostedServices;

    /// <summary>
    /// Constructs a <see cref="ScheduledJobController"/> instance.
    /// </summary>
    public ScheduledJobController(IEnumerable<ICustomHostedService> hostedServices)
    {
        _hostedServices = hostedServices;
    }

    /// <summary>
    /// Runs a specified scheduled job.
    /// </summary>
    /// <param name="jobName">The name of the scheduled job to run.</param>
    /// <returns>OK.</returns>
    [HttpPost("{jobName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RunScheduledJob([FromRoute] string jobName)
    {
        var message = "OK";
        var job = _hostedServices.FirstOrDefault(s =>
            string.Equals(jobName, s.Key, StringComparison.OrdinalIgnoreCase));
        if (job == null)
        {
            throw new NotFoundException($"Hosted service with key '{jobName}'.");
        }

        var statusCode = HttpStatusCode.OK;
        try
        {
            await job.ProcessAsync();
        }
        catch (Exception ex)
        {
            message = ex.ToString();
            statusCode = HttpStatusCode.InternalServerError;
        }

        return StatusCode((int)statusCode, new JobExecutionResultModel(message));
    }
}
