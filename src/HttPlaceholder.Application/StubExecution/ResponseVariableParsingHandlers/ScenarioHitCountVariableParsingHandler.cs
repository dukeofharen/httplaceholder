using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable handler that is used to insert the hit count of a specific scenario in the response
/// </summary>
internal class ScenarioHitCountVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IScenarioStateStore _scenarioStateStore;

    public ScenarioHitCountVariableParsingHandler(IScenarioStateStore scenarioStateStore)
    {
        _scenarioStateStore = scenarioStateStore;
    }

    /// <inheritdoc />
    public string Name => "scenario_hitcount";

    /// <inheritdoc />
    public string FullName => "Scenario hit count";

    /// <inheritdoc />
    public string Example => "((scenario_hitcount:scenario name))";

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var enumerable = matches as Match[] ?? matches.ToArray();
        if (!enumerable.Any())
        {
            return input;
        }

        foreach (var match in enumerable)
        {
            int? hitCount = null;
            var scenarioName = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                ? match.Groups[2].Value
                : stub.Scenario ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(scenarioName))
            {
                var scenario = _scenarioStateStore.GetScenario(scenarioName);
                hitCount = scenario?.HitCount;
            }

            var hitCountText = !hitCount.HasValue ? string.Empty : hitCount.Value.ToString();
            input = input.Replace(match.Value, hitCountText);
        }

        return input;
    }
}
