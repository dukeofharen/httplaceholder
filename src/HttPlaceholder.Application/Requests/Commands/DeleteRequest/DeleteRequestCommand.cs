using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteRequest;

/// <summary>
///     A command for deleting a request.
/// </summary>
public class DeleteRequestCommand : IRequest<bool>
{
    /// <summary>
    ///     Constructs a <see cref="DeleteRequestCommand" /> instance.
    /// </summary>
    /// <param name="correlationId">The request correlation ID.</param>
    public DeleteRequestCommand(string correlationId)
    {
        CorrelationId = correlationId;
    }

    /// <summary>
    ///     Gets the request correlation ID.
    /// </summary>
    public string CorrelationId { get; }
}
