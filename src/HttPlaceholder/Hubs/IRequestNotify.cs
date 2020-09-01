using System.Threading.Tasks;
using HttPlaceholder.Dto.v1.Requests;

namespace HttPlaceholder.Hubs
{
    public interface IRequestNotify
    {
        Task NewRequestReceivedAsync(RequestOverviewDto request);
    }
}
