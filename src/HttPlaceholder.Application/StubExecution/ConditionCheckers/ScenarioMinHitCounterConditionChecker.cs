using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker for validating whether the stub scenario has a minimum (inclusive) number of hits.
/// </summary>
public class ScenarioMinHitCounterConditionChecker : IConditionChecker
{
    private readonly IScenarioService _scenarioService;

    /// <summary>
    /// Constructs a <see cref="ScenarioMinHitCounterConditionChecker"/> instance.
    /// </summary>
    public ScenarioMinHitCounterConditionChecker(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    /// <inheritdoc />
    public ConditionCheckResultModel Validate(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var minHits = stub.Conditions?.Scenario?.MinHits;
        if (minHits == null)
        {
            return result;
        }

        var scenario = stub.Scenario;
        var rawHitCount = _scenarioService.GetHitCount(scenario);
        var actualHitCount = rawHitCount + 1; // Add +1 because the scenario is being hit right now but hit count has not been increased yet.
        if (actualHitCount == null)
        {
            result.Log = "No hit count could be found.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else if (actualHitCount < minHits)
        {
            result.Log =
                $"Scenario '{scenario}' should have at least '{minHits}' hits, but only '{actualHitCount}' hits were counted.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else if (actualHitCount >= minHits)
        {
            result.ConditionValidation = ConditionValidationType.Valid;
        }

        return result;
    }

    /// <inheritdoc />
    public int Priority => 8;
}
