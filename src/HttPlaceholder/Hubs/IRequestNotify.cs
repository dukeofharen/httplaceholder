using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Hubs
{
    public interface IRequestNotify
    {
        Task NewRequestReceivedAsync(RequestResultModel request);
    }
}
