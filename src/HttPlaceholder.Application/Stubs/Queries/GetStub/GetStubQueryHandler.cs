using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Queries.GetStub;

/// <summary>
/// A query handler for retrieving a specific stub.
/// </summary>
public class GetStubQueryHandler : IRequestHandler<GetStubQuery, FullStubModel>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="GetStubQueryHandler"/> instance.
    /// </summary>
    public GetStubQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
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
