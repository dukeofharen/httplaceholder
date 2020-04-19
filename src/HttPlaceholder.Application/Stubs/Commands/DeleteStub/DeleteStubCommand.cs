using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteStub
{
    public class DeleteStubCommand : IRequest
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string StubId { get; set; }
    }
}
