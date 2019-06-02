using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStub
{
    public class GetStubQuery : IRequest<FullStubModel>
    {
        public string StubId { get; set; }
    }
}
