using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Domain;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker for validating whether the stub scenario has an exact number of hits.
/// </summary>
public class ScenarioExactHitCounterConditionChecker(IStubContext stubContext) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public async Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var exactHits = stub.Conditions?.Scenario?.ExactHits;
        if (exactHits == null)
        {
            return await NotExecutedAsync();
        }

        var scenario = stub.Scenario;
        var rawHitCount = await stubContext.GetHitCountAsync(scenario, cancellationToken);
        var actualHitCount =
            rawHitCount +
            1; // Add +1 because the scenario is being hit right now but hit count has not been increased yet.
        if (actualHitCount == null)
        {
            return await InvalidAsync("No hit count could be found.");
        }

        if (actualHitCount != exactHits)
        {
            return await InvalidAsync(
                $"Scenario '{scenario}' should have exactly '{exactHits}' hits, but '{actualHitCount}' hits were counted.");
        }

        return await ValidAsync();
    }

    /// <inheritdoc />
    public int Priority => 8;
}
