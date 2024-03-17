using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the stub scenario is in a specific state.
/// </summary>
public class ScenarioStateConditionChecker(IStubContext stubContext) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var state = stub.Conditions?.Scenario?.ScenarioState;
        var scenario = stub.Scenario;
        if (string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(scenario))
        {
            return result;
        }

        var scenarioState = await stubContext.GetScenarioAsync(scenario, cancellationToken);
        if (scenarioState == null)
        {
            scenarioState = new ScenarioStateModel(scenario);
            await stubContext.SetScenarioAsync(scenario, scenarioState, cancellationToken);
        }

        if (!string.Equals(scenarioState.State.Trim(), state.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            result.Log =
                $"Scenario '{stub.Scenario}' is in state '{scenarioState.State}', but '{state}' was expected.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else
        {
            result.ConditionValidation = ConditionValidationType.Valid;
        }

        return result;
    }

    /// <inheritdoc />
    public int Priority => 8;
}
