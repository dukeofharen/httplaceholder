using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the stub scenario has an exact number of hits.
/// </summary>
public class ScenarioExactHitCounterConditionChecker(IStubContext stubContext) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var exactHits = stub.Conditions?.Scenario?.ExactHits;
        if (exactHits == null)
        {
            return result;
        }

        var scenario = stub.Scenario;
        var rawHitCount = await stubContext.GetHitCountAsync(scenario, cancellationToken);
        var actualHitCount =
            rawHitCount +
            1; // Add +1 because the scenario is being hit right now but hit count has not been increased yet.
        if (actualHitCount == null)
        {
            result.Log = "No hit count could be found.";
            result.ConditionValidation = ConditionValidationType.Invalid;
        }
        else if (actualHitCount != exactHits)
        {
            result.Log =
                $"Scenario '{scenario}' should have exactly '{exactHits}' hits, but '{actualHitCount}' hits were counted.";
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
