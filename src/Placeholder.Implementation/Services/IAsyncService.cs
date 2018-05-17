using System.Threading.Tasks;

namespace Placeholder.Implementation.Services
{
   public interface IAsyncService
   {
      Task DelayAsync(int millis);
   }
}
