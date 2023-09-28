using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the stub scenario has a minimum (inclusive) number of hits.
/// </summary>
public class ScenarioMinHitCounterConditionChecker : IConditionChecker, ISingletonService
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="ScenarioMinHitCounterConditionChecker" /> instance.
    /// </summary>
    public ScenarioMinHitCounterConditionChecker(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var minHits = stub.Conditions?.Scenario?.MinHits;
        if (minHits == null)
        {
            return result;
        }

        var scenario = stub.Scenario;
        var rawHitCount = await _stubContext.GetHitCountAsync(scenario, cancellationToken);
        var actualHitCount =
            rawHitCount +
            1; // Add +1 because the scenario is being hit right now but hit count has not been increased yet.
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
