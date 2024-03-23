using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.Configuration.Models;
using HttPlaceholder.Application.Interfaces.Http;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttPlaceholder.Application.Infrastructure.MediatR;

/// <summary>
///     A MediatR behavior for logging calls to MediatR.
/// </summary>
public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IClientDataResolver _clientDataResolver;
    private readonly ILogger<AuditBehavior<TRequest, TResponse>> _logger;
    private readonly IOptionsMonitor<SettingsModel> _options;

    /// <summary>
    ///     Constructs an <see cref="AuditBehavior{TRequest, TResponse}" /> instance
    /// </summary>
    public AuditBehavior(
        IOptionsMonitor<SettingsModel> options,
        IClientDataResolver clientDataResolver,
        ILogger<AuditBehavior<TRequest, TResponse>> logger)
    {
        _options = options;
        _clientDataResolver = clientDataResolver;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (_options?.CurrentValue?.Logging?.VerboseLoggingEnabled != true)
        {
            return await next();
        }

        var builder = new StringBuilder();
        builder.AppendLine("Audit:");

        var type = request.GetType();
        builder.AppendLine($"Handling request '{type.FullName}'");
        builder.AppendLine($"Input: {JsonConvert.SerializeObject(request)}");

        var ip = _clientDataResolver?.GetClientIp();
        if (ip != null)
        {
            builder.AppendLine($"IP: {ip}");
        }

        try
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await next();
            stopwatch.Stop();
            builder.AppendLine($"Duration: {stopwatch.Elapsed.Milliseconds} ms");
            return result;
        }
        catch (Exception ex)
        {
            builder.AppendLine($"{ex.GetType()} thrown: {ex}");
            throw;
        }
        finally
        {
            _logger.LogInformation(builder.ToString());
        }
    }
}
