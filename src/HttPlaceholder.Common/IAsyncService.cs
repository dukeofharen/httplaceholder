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
    /// <returns>A task.</returns>
    Task DelayAsync(int millis);
}
