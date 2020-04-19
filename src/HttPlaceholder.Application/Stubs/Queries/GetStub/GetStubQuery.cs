using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStub
{
    public class GetStubQuery : IRequest<FullStubModel>
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string StubId { get; set; }
    }
}
