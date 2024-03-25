using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using static HttPlaceholder.Domain.StubResponseWriterResultModel;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to set the state of the stub scenario to another value.
/// </summary>
internal class SetScenarioStateResponseWriter(IStubContext stubContext) : IResponseWriter, ISingletonService
{
    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(stub.Response.Scenario?.SetScenarioState) ||
            string.IsNullOrWhiteSpace(stub.Scenario))
        {
            return IsNotExecuted(GetType().Name);
        }

        var scenario = stub.Scenario;
        var scenarioState = await stubContext.GetScenarioAsync(scenario, cancellationToken) ??
                            new ScenarioStateModel(scenario);

        scenarioState.State = stub.Response.Scenario.SetScenarioState;
        await stubContext.SetScenarioAsync(scenario, scenarioState, cancellationToken);
        return IsExecuted(GetType().Name);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
