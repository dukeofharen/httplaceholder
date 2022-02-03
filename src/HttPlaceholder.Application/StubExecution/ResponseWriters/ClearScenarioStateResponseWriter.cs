using System.Threading.Tasks;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to clear the scenario state of the stub (both hit counter and state).
/// </summary>
internal class ClearScenarioStateResponseWriter : IResponseWriter
{
    private readonly IScenarioService _scenarioService;

    public ClearScenarioStateResponseWriter(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response)
    {
        if (stub.Response.Scenario?.ClearState != true || string.IsNullOrWhiteSpace(stub.Scenario))
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        _scenarioService.DeleteScenario(stub.Scenario);
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }

    /// <inheritdoc />
    public int Priority => 0;
}
