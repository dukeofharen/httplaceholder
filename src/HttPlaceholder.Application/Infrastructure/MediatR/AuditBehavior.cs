using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace HttPlaceholder.Application.Infrastructure.MediatR;

/// <summary>
///     A MediatR behavior for logging calls to MediatR.
/// </summary>
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(request.GetType().FullName);
        return await next();
    }
}
