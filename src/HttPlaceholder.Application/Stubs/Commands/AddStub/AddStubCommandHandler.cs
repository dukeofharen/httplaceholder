using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStub
{
    public class AddStubCommandHandler : IRequestHandler<AddStubCommand, FullStubModel>
    {
        private readonly IStubContext _stubContext;

        public AddStubCommandHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<FullStubModel> Handle(AddStubCommand request, CancellationToken cancellationToken)
        {
            // Delete stub with same ID.
            await _stubContext.DeleteStubAsync(request.Stub.Id);
            return await _stubContext.AddStubAsync(request.Stub);
        }
    }
}
