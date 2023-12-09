using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class StubResponseGenerator(
    IRequestLoggerFactory requestLoggerFactory,
    IEnumerable<IResponseWriter> responseWriters)
    : IStubResponseGenerator, ISingletonService
{
    /// <inheritdoc />
    public async Task<ResponseModel> GenerateResponseAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var requestLogger = requestLoggerFactory.GetRequestLogger();
        var response = new ResponseModel();
        foreach (var writer in responseWriters.OrderByDescending(w => w.Priority))
        {
            var result = await writer.WriteToResponseAsync(stub, response, cancellationToken);
            if (result?.Executed == true)
            {
                requestLogger.SetResponseWriterResult(result);
            }
        }

        return response;
    }
}
