using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the stub scenario is in a specific state.
/// </summary>
public class ScenarioStateConditionChecker(IStubContext stubContext) : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 8;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) =>
        StringHelper.NoneAreNullOrWhitespace(stub.Conditions?.Scenario?.ScenarioState, stub.Scenario);

    /// <inheritdoc />
    protected override async Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var state = stub.Conditions.Scenario.ScenarioState.Trim();
        var scenario = stub.Scenario;
        var scenarioState = await stubContext.GetScenarioAsync(scenario, cancellationToken);
        if (scenarioState == null)
        {
            scenarioState = new ScenarioStateModel(scenario);
            await stubContext.SetScenarioAsync(scenario, scenarioState, cancellationToken);
        }

        if (!string.Equals(scenarioState.State.Trim(), state.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return await InvalidAsync(string.Format(StubResources.ScenarioStateConditionFailed, stub.Scenario,
                scenarioState.State, state));
        }

        return await ValidAsync();
    }
}
