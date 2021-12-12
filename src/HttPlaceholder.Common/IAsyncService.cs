using System.Threading.Tasks;

namespace HttPlaceholder.Common;

public interface IAsyncService
{
    Task DelayAsync(int millis);

    void Sleep(int millis);
}