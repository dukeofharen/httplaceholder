using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc/>
internal class StubResponseGenerator : IStubResponseGenerator
{
    private readonly IRequestLoggerFactory _requestLoggerFactory;
    private readonly IEnumerable<IResponseWriter> _responseWriters;

    public StubResponseGenerator(
        IRequestLoggerFactory requestLoggerFactory,
        IEnumerable<IResponseWriter> responseWriters)
    {
        _requestLoggerFactory = requestLoggerFactory;
        _responseWriters = responseWriters;
    }

    /// <inheritdoc/>
    public async Task<ResponseModel> GenerateResponseAsync(StubModel stub)
    {
        var requestLogger = _requestLoggerFactory.GetRequestLogger();
        var response = new ResponseModel();
        foreach (var writer in _responseWriters.OrderByDescending(w => w.Priority))
        {
            var result = await writer.WriteToResponseAsync(stub, response);
            if (result.Executed)
            {
                requestLogger.SetResponseWriterResult(result);
            }
        }

        return response;
    }
}
