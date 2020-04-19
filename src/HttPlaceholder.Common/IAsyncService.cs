using System.Threading.Tasks;
// ReSharper disable UnusedMember.Global

namespace HttPlaceholder.Common
{
    public interface IAsyncService
    {
        Task DelayAsync(int millis);

        void Sleep(int millis);
    }
}
