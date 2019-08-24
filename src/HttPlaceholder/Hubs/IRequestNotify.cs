using System.Threading.Tasks;
using HttPlaceholder.Domain;
using HttPlaceholder.Dto.Requests;

namespace HttPlaceholder.Hubs
{
    public interface IRequestNotify
    {
        Task NewRequestReceivedAsync(RequestResultDto request);
    }
}
