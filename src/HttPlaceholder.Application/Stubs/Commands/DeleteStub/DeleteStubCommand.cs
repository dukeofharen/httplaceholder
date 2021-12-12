using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteStub;

public class DeleteStubCommand : IRequest
{
    public DeleteStubCommand(string stubId)
    {
        StubId = stubId;
    }

    public string StubId { get; }
}