using System.Threading.Tasks;
using HttPlaceholder.Common;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.HostedServices;

/// <summary>
/// A background that is used to clean old requests
/// </summary>
public class CleanOldRequestsJob : BackgroundService
{
    /// <summary>
    /// Constructs a <see cref="CleanOldRequestsJob"/> instance.
    /// </summary>
    public CleanOldRequestsJob(ILogger<BackgroundService> logger, IDateTime dateTime, IAsyncService asyncService) :
        base(logger, dateTime, asyncService)
    {
    }

    /// <inheritdoc />
    public override string Schedule => "*/5 * * * *";

    /// <inheritdoc />
    public override string Key => "CleanOldRequestsJob";

    /// <inheritdoc />
    public override string Description => "A job for cleaning old requests.";

    /// <inheritdoc />
    public override Task ProcessAsync() => throw new System.NotImplementedException();
}
