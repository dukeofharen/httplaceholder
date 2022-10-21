using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to clear the scenario state of the stub (both hit counter and state).
/// </summary>
internal class ClearScenarioStateResponseWriter : IResponseWriter, ISingletonService
{
    private readonly IScenarioService _scenarioService;

    public ClearScenarioStateResponseWriter(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        if (stub.Response.Scenario?.ClearState != true || string.IsNullOrWhiteSpace(stub.Scenario))
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        await _scenarioService.DeleteScenarioAsync(stub.Scenario, cancellationToken);
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
