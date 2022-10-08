using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

/// <inheritdoc />
public class AsyncService : IAsyncService
{
    /// <inheritdoc />
    public async Task DelayAsync(int millis, CancellationToken cancellationToken) =>
        await Task.Delay(millis, cancellationToken);
}
