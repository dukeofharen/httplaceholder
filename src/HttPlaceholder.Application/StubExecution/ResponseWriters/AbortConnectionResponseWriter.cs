using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// A response writer that is used to abruptly abort a connection with the calling client.
/// </summary>
internal class AbortConnectionResponseWriter : IResponseWriter
{
    private readonly IHttpContextService _httpContextService;

    public AbortConnectionResponseWriter(IHttpContextService httpContextService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        var abortConnection = stub.Response?.AbortConnection == true;
        if (!abortConnection)
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        _httpContextService.AbortConnection();
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }

    /// <inheritdoc />
    public int Priority => 100;

    /// <inheritdoc />
    public bool ShouldStopIfWriterRan => true;
}
