using System;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker for validating whether the stub scenario is in a specific state.
/// </summary>
public class ScenarioStateConditionChecker : IConditionChecker
{
    private readonly IScenarioService _scenarioService;

    /// <summary>
    /// Constructs a <see cref="ScenarioStateConditionChecker"/> instance.
    /// </summary>
    public ScenarioStateConditionChecker(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

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

        var scenarioState = _scenarioService.GetScenario(scenario);
        if (scenarioState == null)
        {
            scenarioState = new ScenarioStateModel(scenario);
            await _scenarioService.SetScenarioAsync(scenario, scenarioState, cancellationToken);
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
