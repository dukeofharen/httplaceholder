using System.Threading.Tasks;

namespace Budgetkar.Services.Implementations
{
    internal class AsyncService : IAsyncService
    {
       public async Task DelayAsync(int millis)
       {
          await Task.Delay(millis);
       }
    }
}
