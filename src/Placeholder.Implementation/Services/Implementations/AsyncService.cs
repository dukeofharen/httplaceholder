using System.Threading.Tasks;

namespace Placeholder.Implementation.Services.Implementations
{
    internal class AsyncService : IAsyncService
    {
       public async Task DelayAsync(int millis)
       {
          await Task.Delay(millis);
       }
    }
}
