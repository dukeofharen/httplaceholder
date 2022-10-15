using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
/// Response writer that is used to set the state of the stub scenario to another value.
/// </summary>
internal class SetScenarioStateResponseWriter : IResponseWriter, ISingletonService
{
    private readonly IScenarioService _scenarioService;

    public SetScenarioStateResponseWriter(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    /// <inheritdoc />
    public Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(stub.Response.Scenario?.SetScenarioState) ||
            string.IsNullOrWhiteSpace(stub.Scenario))
        {
            return Task.FromResult(StubResponseWriterResultModel.IsNotExecuted(GetType().Name));
        }

        var scenario = stub.Scenario;
        var scenarioState = _scenarioService.GetScenario(scenario) ?? new ScenarioStateModel(scenario);

        scenarioState.State = stub.Response.Scenario.SetScenarioState;
        _scenarioService.SetScenarioAsync(scenario, scenarioState, cancellationToken);
        return Task.FromResult(StubResponseWriterResultModel.IsExecuted(GetType().Name));
    }

    /// <inheritdoc />
    public int Priority => 0;
}
