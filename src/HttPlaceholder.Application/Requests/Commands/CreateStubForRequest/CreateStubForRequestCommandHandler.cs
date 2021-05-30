using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Commands.CreateStubForRequest
{
    public class CreateStubForRequestCommandHandler : IRequestHandler<CreateStubForRequestCommand, FullStubModel>
    {
        private readonly IRequestStubGenerator _requestStubGenerator;

        public CreateStubForRequestCommandHandler(IRequestStubGenerator requestStubGenerator)
        {
            _requestStubGenerator = requestStubGenerator;
        }

        public async Task<FullStubModel> Handle(
            CreateStubForRequestCommand request,
            CancellationToken cancellationToken) =>
            await _requestStubGenerator.GenerateStubBasedOnRequestAsync(request.CorrelationId, request.DoNotCreateStub);
    }
}
