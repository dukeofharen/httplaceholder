using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.DeleteStub
{
    public class DeleteStubCommand : IRequest
    {
        public DeleteStubCommand(string stubId)
        {
            StubId = stubId;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string StubId { get; }
    }
}
