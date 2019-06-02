using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStub
{
    public class GetStubQueryHandler : IRequestHandler<GetStubQuery, FullStubModel>
    {
        private readonly IStubContext _stubContext;

        public GetStubQueryHandler(IStubContext stubContext)
        {
            _stubContext = stubContext;
        }

        public async Task<FullStubModel> Handle(GetStubQuery request, CancellationToken cancellationToken)
        {
            var result = await _stubContext.GetStubAsync(request.StubId);
            if (result == null)
            {
                throw new NotFoundException(nameof(StubModel), request.StubId);
            }

            return result;
        }
    }
}
