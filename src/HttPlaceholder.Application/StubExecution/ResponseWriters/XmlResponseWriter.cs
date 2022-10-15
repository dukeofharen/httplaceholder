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
internal class XmlResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response, CancellationToken cancellationToken)
    {
        if (stub.Response?.Xml == null)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        var body = stub.Response.Xml;
        response.Body = Encoding.UTF8.GetBytes(body);
        response.Headers.AddOrReplaceCaseInsensitive("Content-Type", Constants.XmlTextMime, false);

        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}
