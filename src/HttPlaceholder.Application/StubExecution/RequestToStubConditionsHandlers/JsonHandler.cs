using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     This handler is used to check whether the request contains JSON.
///     The content type should be JSON, the JSON should be correct and no request body should have been set for the stub
///     yet.
///     JSON objects and arrays are supported as root node.
/// </summary>
internal class JsonHandler : IRequestToStubConditionsHandler, ISingletonService
{
    private readonly ILogger<JsonHandler> _logger;

    public JsonHandler(ILogger<JsonHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        var pair = request.Headers.FirstOrDefault(p =>
            p.Key.Equals(HeaderKeys.ContentType, StringComparison.OrdinalIgnoreCase));
        var contentType = pair.Value;
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return Task.FromResult(false);
        }

        var supportedContentTypes = new[] {Constants.JsonMime};
        if (!supportedContentTypes.Any(sc => contentType.StartsWith(sc, StringComparison.OrdinalIgnoreCase)))
        {
            return Task.FromResult(false);
        }

        try
        {
            conditions.Json = JToken.Parse(request.Body);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Exception occurred while trying to parse JSON.");
            return Task.FromResult(false);
        }
    }

    /// <inheritdoc />
    public int Priority => 2;
}
