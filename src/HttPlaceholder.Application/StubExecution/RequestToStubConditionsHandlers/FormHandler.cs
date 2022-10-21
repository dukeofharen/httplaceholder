using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace HttPlaceholder.Application.StubExecution.RequestToStubConditionsHandlers;

/// <summary>
///     "Request to stub conditions handler" that is used to create form conditions.
/// </summary>
internal class FormHandler : IRequestToStubConditionsHandler, ISingletonService
{
    /// <inheritdoc />
    public Task<bool> HandleStubGenerationAsync(HttpRequestModel request, StubConditionsModel conditions,
        CancellationToken cancellationToken)
    {
        var pair = request.Headers.FirstOrDefault(p =>
            p.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase));
        var contentType = pair.Value;
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return Task.FromResult(false);
        }

        var supportedContentTypes = new[] {Constants.UrlEncodedFormMime, Constants.MultipartFormDataMime};
        if (
            !supportedContentTypes.Any(sc => contentType.StartsWith(sc, StringComparison.OrdinalIgnoreCase)) ||
            string.IsNullOrWhiteSpace(request.Body))
        {
            return Task.FromResult(false);
        }

        var reader = new FormReader(request.Body);
        var form = reader.ReadForm();
        if (!form.Any())
        {
            return Task.FromResult(false);
        }

        // If the body condition is already set, clear it here.
        conditions.Body = Array.Empty<string>();

        conditions.Form = form.Select(f => new StubFormModel
        {
            Key = f.Key, Value = new StubConditionStringCheckingModel {StringEquals = f.Value}
        });

        return Task.FromResult(true);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
