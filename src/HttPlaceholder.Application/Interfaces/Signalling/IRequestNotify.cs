using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.Interfaces.Signalling;

/// <summary>
/// Describes a class that is used to send messages to a request hub after a request has been received.
/// </summary>
public interface IRequestNotify
{
    /// <summary>
    /// Handle the receiving of a new request.
    /// </summary>
    /// <param name="request">The request.</param>
    Task NewRequestReceivedAsync(RequestResultModel request);
}
