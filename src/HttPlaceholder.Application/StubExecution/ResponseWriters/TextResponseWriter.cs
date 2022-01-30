using System.Text;
using System.Threading.Tasks;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to return the given response as plain text.
/// </summary>
internal class TextResponseWriter : IResponseWriter
{
    /// <inheritdoc />
    public int Priority => 0;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (stub.Response.Text == null)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        response.Body = Encoding.UTF8.GetBytes(stub.Response.Text);
        response.Headers.AddOrReplaceCaseInsensitive("Content-Type", Constants.TextMime, false);

        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }
}
