using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Requests.Queries.GetResponse;

/// <summary>
/// A query handler for retrieving a response.
/// </summary>
public class GetResponseQueryHandler : IRequestHandler<GetResponseQuery, ResponseModel>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="GetResponseQueryHandler"/> instance.
    /// </summary>
    public GetResponseQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<ResponseModel> Handle(GetResponseQuery request, CancellationToken cancellationToken)
    {
        var result = await _stubContext.GetResponseAsync(request.CorrelationId, cancellationToken);
        if (result == null)
        {
            throw new NotFoundException("response", request.CorrelationId);
        }

        return result;
    }
}
