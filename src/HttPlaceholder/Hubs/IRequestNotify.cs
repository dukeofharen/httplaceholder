using System.Threading.Tasks;
using HttPlaceholder.Models;

namespace HttPlaceholder.Hubs
{
    public interface IRequestNotify
    {
        Task NewRequestReceivedAsync(RequestResultModel request);
    }
}
