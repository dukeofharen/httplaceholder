using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStub
{
    public class AddStubCommand : IRequest<FullStubModel>
    {
        public StubModel Stub { get; set; }
    }
}
