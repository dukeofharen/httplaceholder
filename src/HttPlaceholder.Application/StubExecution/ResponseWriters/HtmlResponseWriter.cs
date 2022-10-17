using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to return the given response as HTML.
/// </summary>
internal class HtmlResponseWriter : BaseBodyResponseWriter, ISingletonService
{
    /// <inheritdoc />
    protected override string GetContentType() => Constants.HtmlMime;

    /// <inheritdoc />
    protected override string GetBodyFromStub(StubModel stub) => stub.Response?.Html;

    /// <inheritdoc />
    protected override string GetWriterName() => GetType().Name;

    /// <inheritdoc />
    public override int Priority => 0;
}
