using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to to a string or regex replace in the response.
/// </summary>
public class StringReplaceResponseWriter : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public int Priority => -11;

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        var replace = stub.Response?.Replace?.ToArray();
        if (replace == null || !replace.Any() || response.Body == null || !response.Body.Any())
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        var body = Encoding.UTF8.GetString(response.Body);
        body = replace.Aggregate(body, PerformReplace);
        response.Body = Encoding.UTF8.GetBytes(body);

        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }

    private static string PerformReplace(string body, StubResponseReplaceModel model)
    {
        // TODO add option to ignore case.
        return body.Replace(model.Text, model.ReplaceWith);
    }
}
