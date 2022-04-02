using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequest;

/// <summary>
/// A query handler for retrieving a request.
/// </summary>
public class GetRequestQueryHandler : IRequestHandler<GetRequestQuery, RequestResultModel>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="GetRequestQueryHandler"/> instance.
    /// </summary>
    public GetRequestQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<RequestResultModel> Handle(GetRequestQuery request, CancellationToken cancellationToken)
    {
        var result = await _stubContext.GetRequestResultAsync(request.CorrelationId);
        if (result == null)
        {
            throw new NotFoundException("request", request.CorrelationId);
        }

        return result;
    }
}
