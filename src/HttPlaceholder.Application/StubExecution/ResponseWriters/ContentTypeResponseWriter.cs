using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to set the Content-Type header.
/// </summary>
public class ContentTypeResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(stub.Response?.ContentType))
        {
            return Task.FromResult(IsNotExecuted(GetType().Name));
        }

        response.Headers.AddOrReplaceCaseInsensitive(HeaderKeys.ContentType, stub.Response.ContentType);
        return Task.FromResult(IsExecuted(GetType().Name));
    }

    /// <inheritdoc />
    public int Priority => -11;
}
