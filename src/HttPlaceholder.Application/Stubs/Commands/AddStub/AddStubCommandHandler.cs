using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.AddStub
{
    public class AddStubCommandHandler : IRequestHandler<AddStubCommand, FullStubModel>
    {
        private readonly IStubContext _stubContext;
        private readonly IStubModelValidator _stubModelValidator;

        public AddStubCommandHandler(IStubContext stubContext, IStubModelValidator stubModelValidator)
        {
            _stubContext = stubContext;
            _stubModelValidator = stubModelValidator;
        }

        public async Task<FullStubModel> Handle(AddStubCommand request, CancellationToken cancellationToken)
        {
            var validationResults = _stubModelValidator.ValidateStubModel(request.Stub);
            if (validationResults.Any())
            {
                throw new ValidationException(validationResults);
            }

            // Delete stub with same ID.
            await _stubContext.DeleteStubAsync(request.Stub.Id);
            return await _stubContext.AddStubAsync(request.Stub);
        }
    }
}
