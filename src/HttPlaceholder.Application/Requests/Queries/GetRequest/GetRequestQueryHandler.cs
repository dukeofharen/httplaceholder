using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetRequest;

public class GetRequestQueryHandler : IRequestHandler<GetRequestQuery, RequestResultModel>
{
    private readonly IStubContext _stubContext;

    public GetRequestQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

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