using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to return a given response as JSON.
/// </summary>
internal class JsonResponseWriter : BaseBodyResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 0;

    /// <inheritdoc />
    protected override string GetContentType() => Constants.JsonMime;

    /// <inheritdoc />
    protected override string GetBodyFromStub(StubModel stub) => stub.Response?.Json;

    /// <inheritdoc />
    protected override string GetWriterName() => GetType().Name;
}
