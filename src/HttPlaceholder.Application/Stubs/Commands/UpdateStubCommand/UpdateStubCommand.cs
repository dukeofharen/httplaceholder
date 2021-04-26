using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand
{
    public class UpdateStubCommand : IRequest
    {
        public UpdateStubCommand(string stubId, StubModel stub)
        {
            StubId = stubId;
            Stub = stub;
        }

        public string StubId { get; }

        public StubModel Stub { get; }
    }
}
