using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.DeleteRequest;

public class DeleteRequestCommand : IRequest<bool>
{
    public DeleteRequestCommand(string correlationId)
    {
        CorrelationId = correlationId;
    }

    public string CorrelationId { get; }
}