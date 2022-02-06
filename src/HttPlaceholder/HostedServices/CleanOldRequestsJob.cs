using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.HostedServices;

/// <summary>
/// A background that is used to clean old requests
/// </summary>
public class CleanOldRequestsJob : BackgroundService
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="CleanOldRequestsJob"/> instance.
    /// </summary>
    public CleanOldRequestsJob(ILogger<BackgroundService> logger, IDateTime dateTime, IAsyncService asyncService,
        IStubContext stubContext) :
        base(logger, dateTime, asyncService)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public override string Schedule => "*/5 * * * *";

    /// <inheritdoc />
    public override string Key => "CleanOldRequestsJob";

    /// <inheritdoc />
    public override string Description => "A job for cleaning old requests.";

    /// <inheritdoc />
    public override async Task ProcessAsync()
    {
        Logger.LogDebug("Cleaning old requests.");
        await _stubContext.CleanOldRequestResultsAsync();
    }
}
