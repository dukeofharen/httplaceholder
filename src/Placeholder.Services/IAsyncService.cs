using System.Threading.Tasks;

namespace Budgetkar.Services
{
   public interface IAsyncService
   {
      Task DelayAsync(int millis);
   }
}
