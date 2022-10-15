using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

internal class AsyncService : IAsyncService, ISingletonService
{
    /// <inheritdoc />
    public async Task DelayAsync(int millis, CancellationToken cancellationToken) =>
        await Task.Delay(millis, cancellationToken);
}
