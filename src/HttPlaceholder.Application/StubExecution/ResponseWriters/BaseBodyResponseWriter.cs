using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     An abstract class that is used to return the response body in a specific format.
/// </summary>
public abstract class BaseBodyResponseWriter : IResponseWriter
{
    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(
        StubModel stub,
        ResponseModel response,
        CancellationToken cancellationToken)
    {
        var body = GetBodyFromStub(stub);
        if (body == null)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetWriterName()));
        }

        response.BodyIsBinary = false;
        response.Body = Encoding.UTF8.GetBytes(body);
        response.Headers.AddOrReplaceCaseInsensitive(Constants.ContentType, GetContentType(), false);

        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetWriterName()));
    }

    /// <inheritdoc />
    public abstract int Priority { get; }

    /// <summary>
    ///     Retrieves the content type.
    /// </summary>
    /// <returns>The content type.</returns>
    protected abstract string GetContentType();

    /// <summary>
    ///     Retrieves the response body from the stub.
    /// </summary>
    /// <param name="stub">The stub.</param>
    /// <returns>The response body.</returns>
    protected abstract string GetBodyFromStub(StubModel stub);

    /// <summary>
    ///     Retrieves the name from the response writer.
    /// </summary>
    /// <returns>The name of the response writer.</returns>
    protected abstract string GetWriterName();
}
