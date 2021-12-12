using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common;

namespace HttPlaceholder.Infrastructure.Implementations;

public class AsyncService : IAsyncService
{
    public async Task DelayAsync(int millis) =>
        await Task.Delay(millis);

    public void Sleep(int millis) =>
        Thread.Sleep(millis);
}