using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStub
{
    // ReSharper disable once UnusedType.Global
    public class AddStubCommandHandler : IRequestHandler<AddStubCommand>
    {
        private readonly IStubContext _stubContext;

        public AddStubCommandHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<Unit> Handle(AddStubCommand request, CancellationToken cancellationToken)
        {
            // Delete stub with same ID.
            await _stubContext.DeleteStubAsync(request.Stub.Id);

            await _stubContext.AddStubAsync(request.Stub);

            return Unit.Value;
        }
    }
}
