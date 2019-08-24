using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand
{
    public class UpdateStubCommandHandler : IRequestHandler<UpdateStubCommand>
    {
        private readonly IStubContext _stubContext;

        public UpdateStubCommandHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<Unit> Handle(UpdateStubCommand request, CancellationToken cancellationToken)
        {
            // Delete stub with same ID.
            await _stubContext.DeleteStubAsync(request.StubId);
            await _stubContext.DeleteStubAsync(request.Stub.Id);

            await _stubContext.AddStubAsync(request.Stub);

            return Unit.Value;
        }
    }
}
