using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to return the given response as XML.
/// </summary>
internal class XmlResponseWriter : BaseBodyResponseWriter, ISingletonService
{
    /// <inheritdoc />
    protected override string GetContentType() => Constants.XmlTextMime;

    /// <inheritdoc />
    protected override string GetBodyFromStub(StubModel stub) => stub.Response?.Xml;

    /// <inheritdoc />
    protected override string GetWriterName() => GetType().Name;

    /// <inheritdoc />
    public override int Priority => 0;
}
