using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to run the "response variable parsing handlers" for manipulating the response.
/// </summary>
internal class DynamicResponseWriter(IResponseVariableParser responseVariableParser)
    : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => -12;

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        if (stub.Response.EnableDynamicMode != true)
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        // Try to parse and replace the variables in the body.
        if (!response.BodyIsBinary && response.Body != null)
        {
            var parsedBody =
                await responseVariableParser.ParseAsync(Encoding.UTF8.GetString(response.Body), stub,
                    cancellationToken);
            response.Body = Encoding.UTF8.GetBytes(parsedBody);
        }

        // Try to parse and replace the variables in the header values.
        var keys = response.Headers.Keys.ToArray();
        foreach (var key in keys)
        {
            response.Headers[key] =
                await responseVariableParser.ParseAsync(response.Headers[key], stub, cancellationToken);
        }

        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }
}
