using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStub
{
    public class GetStubQuery : IRequest<FullStubModel>
    {
        public GetStubQuery(string stubId)
        {
            StubId = stubId;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string StubId { get; }
    }
}
