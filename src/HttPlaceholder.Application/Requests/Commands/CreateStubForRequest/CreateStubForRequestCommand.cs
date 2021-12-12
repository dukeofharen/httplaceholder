using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.CreateStubForRequest;

public class CreateStubForRequestCommand : IRequest<FullStubModel>
{
    public CreateStubForRequestCommand(string correlationId, bool doNotCreateStub)
    {
        CorrelationId = correlationId;
        DoNotCreateStub = doNotCreateStub;
    }

    public string CorrelationId { get; }

    public bool DoNotCreateStub { get; }
}