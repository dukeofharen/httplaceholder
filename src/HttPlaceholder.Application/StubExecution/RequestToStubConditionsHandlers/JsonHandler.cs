﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Common.Utilities;
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
internal class JsonHandler(ILogger<JsonHandler> logger) : IRequestToStubConditionsHandler, ISingletonService
{
    private static readonly string[] _supportedContentTypes = [MimeTypes.JsonMime];

    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        var pair = request.Headers.FirstOrDefault(p =>
            p.Key.Equals(HeaderKeys.ContentType, StringComparison.OrdinalIgnoreCase));
        var contentType = pair.Value;
        if (
            string.IsNullOrWhiteSpace(contentType) ||
            !_supportedContentTypes.Any(sc => contentType.StartsWith(sc, StringComparison.OrdinalIgnoreCase)))
        {
            return false.AsTask();
        }

        try
        {
            conditions.Json = JToken.Parse(request.Body);
            return true.AsTask();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception occurred while trying to parse JSON.");
            return false.AsTask();
        }
    }

    /// <inheritdoc />
    public int Priority => 2;
}
