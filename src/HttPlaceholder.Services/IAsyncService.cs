using System.Threading.Tasks;

namespace HttPlaceholder.Services
{
    public interface IAsyncService
    {
        Task DelayAsync(int millis);
    }
}