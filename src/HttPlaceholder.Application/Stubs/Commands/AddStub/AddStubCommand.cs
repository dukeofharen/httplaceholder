using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStub
{
    public class AddStubCommand : IRequest
    {
        public StubModel Stub { get; set; }
    }
}
