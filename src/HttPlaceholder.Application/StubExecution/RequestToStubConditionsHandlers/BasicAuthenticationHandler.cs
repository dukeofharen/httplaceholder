using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     "Request to stub conditions handler" that is used to create a basic authentication condition.
/// </summary>
internal class BasicAuthenticationHandler : IRequestToStubConditionsHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        var pair = request.Headers.FirstOrDefault(p =>
            p.Key.Equals(HeaderKeys.Authorization, StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrWhiteSpace(pair.Value) ||
            !pair.Value.Trim().ToLower().StartsWith("Basic", StringComparison.OrdinalIgnoreCase))
        {
            return false.AsTask();
        }

        var basicAuth = pair.Value.Replace("Basic ", string.Empty).Base64Decode();
        var parts = basicAuth.Split(':');
        if (parts.Length != 2)
        {
            return false.AsTask();
        }

        conditions.BasicAuthentication = new StubBasicAuthenticationModel { Username = parts[0], Password = parts[1] };

        // Make sure the original Authorization header is removed here.
        conditions.Headers = conditions.Headers
            .Where(h => !h.Key.Equals(HeaderKeys.Authorization, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(d => d.Key, d => d.Value);

        return true.AsTask();
    }

    /// <inheritdoc />
    public int Priority => 0;
}
