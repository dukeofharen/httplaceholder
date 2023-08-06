using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using MediatR;
using Microsoft.Extensions.Options;

namespace HttPlaceholder.Application.Infrastructure.MediatR;

/// <summary>
///     A MediatR behavior for logging calls to MediatR.
/// </summary>
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IOptionsMonitor<SettingsModel> _options;

    /// <summary>
    ///     Constructs an <see cref="AuditBehavior{TRequest, TResponse}"/> instance
    /// </summary>
    public AuditBehavior(IOptionsMonitor<SettingsModel> options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(request.GetType().FullName);
        if (_options?.CurrentValue?.Logging?.VerboseLoggingEnabled == false)
        {
            return await next();
        }

        return await next();
    }
}
