using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStub;

public class GetStubQuery : IRequest<FullStubModel>
{
    public GetStubQuery(string stubId)
    {
        StubId = stubId;
    }

    public string StubId { get; }
}