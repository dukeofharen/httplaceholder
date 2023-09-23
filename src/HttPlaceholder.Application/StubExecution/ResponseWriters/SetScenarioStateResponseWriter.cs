using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution.ResponseWriters;

/// <summary>
///     Response writer that is used to set the state of the stub scenario to another value.
/// </summary>
internal class SetScenarioStateResponseWriter : IResponseWriter, ISingletonService
{
    private readonly IStubContext _stubContext;

    public SetScenarioStateResponseWriter(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<StubResponseWriterResultModel> WriteToResponseAsync(StubModel stub, ResponseModel response,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(stub.Response.Scenario?.SetScenarioState) ||
            string.IsNullOrWhiteSpace(stub.Scenario))
        {
            return StubResponseWriterResultModel.IsNotExecuted(GetType().Name);
        }

        var scenario = stub.Scenario;
        var scenarioState = await _stubContext.GetScenarioAsync(scenario, cancellationToken) ?? new ScenarioStateModel(scenario);

        scenarioState.State = stub.Response.Scenario.SetScenarioState;
        await _stubContext.SetScenarioAsync(scenario, scenarioState, cancellationToken);
        return StubResponseWriterResultModel.IsExecuted(GetType().Name);
    }

    /// <inheritdoc />
    public int Priority => 0;
}
