using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Domain.Enums;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the stub scenario is in a specific state.
/// </summary>
public class ScenarioStateConditionChecker(IStubContext stubContext) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var state = stub.Conditions?.Scenario?.ScenarioState;
        var scenario = stub.Scenario;
        if (StringHelper.AnyAreNullOrWhitespace(state, scenario))
        {
            return await NotExecutedAsync();
        }

        var scenarioState = await stubContext.GetScenarioAsync(scenario, cancellationToken);
        if (scenarioState == null)
        {
            scenarioState = new ScenarioStateModel(scenario);
            await stubContext.SetScenarioAsync(scenario, scenarioState, cancellationToken);
        }

        if (!string.Equals(scenarioState.State.Trim(), state?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return await InvalidAsync(
                $"Scenario '{stub.Scenario}' is in state '{scenarioState.State}', but '{state}' was expected.");
        }

        return await ValidAsync();
    }

    /// <inheritdoc />
    public int Priority => 8;
}
