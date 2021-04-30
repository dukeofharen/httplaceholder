using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStub
{
    public class AddStubCommand : IRequest<FullStubModel>
    {
        public AddStubCommand(StubModel stub)
        {
            Stub = stub;
        }

        public StubModel Stub { get; }
    }
}
