using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

/// <inheritdoc />
public class HttpResponseToStubResponseService : IHttpResponseToStubResponseService
{
    private readonly IEnumerable<IResponseToStubResponseHandler> _handlers;
    private readonly ILogger<HttpResponseToStubResponseService> _logger;

    public HttpResponseToStubResponseService(
        IEnumerable<IResponseToStubResponseHandler> handlers,
        ILogger<HttpResponseToStubResponseService> logger)
    {
        _handlers = handlers;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<StubResponseModel> ConvertToResponseAsync(HttpResponseModel response)
    {
        var stubResponse = new StubResponseModel();
        foreach (var handler in _handlers.OrderByDescending(h => h.Priority))
        {
            var executed =
                await handler.HandleStubGenerationAsync(response, stubResponse);
            _logger.LogDebug(
                $"Handler '{handler.GetType().Name}' " + (executed ? "executed" : "not executed") + ".");
        }

        return stubResponse;
    }
}
