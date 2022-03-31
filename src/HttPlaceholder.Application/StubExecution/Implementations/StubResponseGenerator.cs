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
    private readonly IStubContext _stubContext;
    private readonly SettingsModel _settings;

    public StubResponseGenerator(
        IRequestLoggerFactory requestLoggerFactory,
        IEnumerable<IResponseWriter> responseWriters,
        IOptions<SettingsModel> options,
        IStubContext stubContext)
    {
        _requestLoggerFactory = requestLoggerFactory;
        _responseWriters = responseWriters;
        _stubContext = stubContext;
        _settings = options.Value;
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

        if (_settings.Storage?.StoreResponses == true)
        {
            await _stubContext.SaveResponseAsync(response);
        }

        return response;
    }
}
