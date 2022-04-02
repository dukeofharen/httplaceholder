using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.Configuration;
using HttPlaceholder.Application.StubExecution.ResponseWriters;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Options;

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
            if (result?.Executed == true)
            {
                requestLogger.SetResponseWriterResult(result);
            }
        }

        return response;
    }
}
