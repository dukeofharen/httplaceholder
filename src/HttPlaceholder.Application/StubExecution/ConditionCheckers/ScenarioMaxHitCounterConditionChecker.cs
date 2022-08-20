using System.Threading.Tasks;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker for validating whether the stub scenario has a maximum (exclusive) number of hits.
/// </summary>
public class ScenarioMaxHitCounterConditionChecker : IConditionChecker
{
    private readonly IScenarioService _scenarioService;

    /// <summary>
    /// Constructs a <see cref="ScenarioMaxHitCounterConditionChecker"/> instance.
    /// </summary>
    public ScenarioMaxHitCounterConditionChecker(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var maxHits = stub.Conditions?.Scenario?.MaxHits;
        if (maxHits == null)
        {
            return Task.FromResult(result);
        }

        var scenario = stub.Scenario;
        var rawHitCount = _scenarioService.GetHitCount(scenario);
        var actualHitCount = rawHitCount + 1; // Add +1 because the scenario is being hit right now but hit count has not been increased yet.
        if (actualHitCount == null)
        {
            result.Log = "No hit count could be found.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else if (actualHitCount >= maxHits)
        {
            result.Log =
                $"Scenario '{scenario}' should have less than '{maxHits}' hits, but '{actualHitCount}' hits were counted.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else if (actualHitCount < maxHits)
        {
            result.ConditionValidation = ConditionValidationType.Valid;
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 8;
}
