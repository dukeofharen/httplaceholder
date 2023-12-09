using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.StubExecution.Models;
using HttPlaceholder.Application.StubExecution.ResponseToStubResponseHandlers;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.Implementations;

internal class HttpResponseToStubResponseService(
    IEnumerable<IResponseToStubResponseHandler> handlers,
    ILogger<HttpResponseToStubResponseService> logger)
    : IHttpResponseToStubResponseService, ISingletonService
{
    /// <inheritdoc />
    public async Task<StubResponseModel> ConvertToResponseAsync(HttpResponseModel response,
        CancellationToken cancellationToken)
    {
        var stubResponse = new StubResponseModel();
        foreach (var handler in handlers.OrderByDescending(h => h.Priority))
        {
            var executed =
                await handler.HandleStubGenerationAsync(response, stubResponse, cancellationToken);
            logger.LogDebug(
                $"Handler '{handler.GetType().Name}' " + (executed ? "executed" : "not executed") + ".");
        }

        return stubResponse;
    }
}
