using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteStub
{
    public class DeleteStubCommand : IRequest
    {
        public string StubId { get; set; }
    }
}
