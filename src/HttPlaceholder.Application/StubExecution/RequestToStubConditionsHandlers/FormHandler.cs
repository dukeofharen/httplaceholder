using System;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <inheritdoc />
internal class FormHandler : IRequestToStubConditionsHandler
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions)
    {
        var pair = request.Headers.FirstOrDefault(p =>
            p.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
        var contentType = pair.Value;
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return Task.FromResult(false);
        }

        var supportedContentTypes = new[] { "application/x-www-form-urlencoded", "multipart/form-data" };
        if (!supportedContentTypes.Any(sc => contentType.StartsWith(sc, StringComparison.OrdinalIgnoreCase)))
        {
            return Task.FromResult(false);
        }

        // If the body condition is already set, clear it here.
        conditions.Body = Array.Empty<string>();

        var reader = new FormReader(request.Body);
        var form = reader.ReadForm();
        conditions.Form = form.Select(f => new StubFormModel
        {
            Key = f.Key,
            Value = f.Value
        });

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}