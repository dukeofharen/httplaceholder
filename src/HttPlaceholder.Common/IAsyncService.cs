using System.Threading;
using System.Threading.Tasks;

namespace HttPlaceholder.Common;

/// <summary>
/// Describes a class that is used to work with asynchronous features.
/// </summary>
public interface IAsyncService
{
    /// <summary>
    /// Starts a delay.
    /// </summary>
    /// <param name="millis">The number of milliseconds to wait.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    Task DelayAsync(int millis, CancellationToken cancellationToken);
}
