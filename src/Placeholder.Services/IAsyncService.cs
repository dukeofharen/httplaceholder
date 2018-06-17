using System.Threading.Tasks;

namespace Placeholder.Services
{
   public interface IAsyncService
   {
      Task DelayAsync(int millis);
   }
}
