using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable handler that is used to insert the hit count of a specific scenario in the response
/// </summary>
internal class ScenarioHitCountVariableParsingHandler(
    IStubContext stubContext,
    ICacheService cacheService)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "scenario_hitcount";

    /// <inheritdoc />
    public override string FullName => ResponseVariableParsingResources.ScenarioHitcount;

    public override string[] Examples => [$"(({Name}))", $"(({Name}:scenario name))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.ScenarioHitcountDescription;

    /// <inheritdoc />
    protected override async Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var result = input;
        var filteredMatches = matches
            .Where(match => match.Groups.Count >= 2);
        foreach (var filteredMatch in filteredMatches)
        {
            result = await InsertHitCountAsync(result, filteredMatch, stub, cancellationToken);
        }

        return result;
    }


    private async Task<string> InsertHitCountAsync(string current, Match match, StubModel stub,
        CancellationToken cancellationToken)
    {
        int? hitCount = null;
        var customScenarioNameSet = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value);
        if (!customScenarioNameSet)
        {
            // Try to read the scenario state from the HttpContext as it contains the correct state of the moment the state was set.
            var state = cacheService.GetScopedItem<ScenarioStateModel>(CachingKeys.ScenarioState);
            hitCount = state?.HitCount;
        }

        if (hitCount == null)
        {
            var scenarioName = StringHelper.GetFirstNonWhitespaceString(match.Groups[2].Value, stub.Scenario);
            if (!string.IsNullOrWhiteSpace(scenarioName))
            {
                var scenario = await stubContext.GetScenarioAsync(scenarioName, cancellationToken);
                hitCount = scenario?.HitCount;
            }
        }

        var hitCountText = !hitCount.HasValue ? string.Empty : hitCount.Value.ToString();
        return current.Replace(match.Value, hitCountText);
    }
}
