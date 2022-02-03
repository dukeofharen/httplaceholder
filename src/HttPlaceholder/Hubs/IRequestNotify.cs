using System.Threading.Tasks;
using HttPlaceholder.Dto.v1.Requests;

namespace HttPlaceholder.Hubs;

/// <summary>
/// Describes a class that is used to send messages to a request SignalR hub after a request has been received.
/// </summary>
public interface IRequestNotify
{
    /// <summary>
    /// Handle the receiving of a new request.
    /// </summary>
    /// <param name="request">The request.</param>
    Task NewRequestReceivedAsync(RequestOverviewDto request);
}
