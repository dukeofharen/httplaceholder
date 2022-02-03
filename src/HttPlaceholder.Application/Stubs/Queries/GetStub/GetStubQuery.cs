using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStub;

/// <summary>
/// A query for retrieving a specific stub.
/// </summary>
public class GetStubQuery : IRequest<FullStubModel>
{
    /// <summary>
    /// Constructs a <see cref="GetStubQuery"/> instance.
    /// </summary>
    /// <param name="stubId">The stub ID.</param>
    public GetStubQuery(string stubId)
    {
        StubId = stubId;
    }

    /// <summary>
    /// Gets the stub ID.
    /// </summary>
    public string StubId { get; }
}
