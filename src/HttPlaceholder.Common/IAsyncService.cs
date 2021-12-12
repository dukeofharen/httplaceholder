using System.Threading.Tasks;

namespace HttPlaceholder.Common;

public interface IAsyncService
{
    Task DelayAsync(int millis);
}
