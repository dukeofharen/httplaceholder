using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

public class ScenarioMinHitCounterConditionChecker : IConditionChecker
{
    private readonly IScenarioService _scenarioService;

    public ScenarioMinHitCounterConditionChecker(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

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

    public int Priority => 8;
}