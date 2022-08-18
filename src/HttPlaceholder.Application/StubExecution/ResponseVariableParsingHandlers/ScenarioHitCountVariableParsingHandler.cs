using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable handler that is used to insert the hit count of a specific scenario in the response
/// </summary>
internal class ScenarioHitCountVariableParsingHandler : BaseVariableParsingHandler
{
    private readonly IScenarioStateStore _scenarioStateStore;

    public ScenarioHitCountVariableParsingHandler(IScenarioStateStore scenarioStateStore, IFileService fileService) :
        base(fileService)
    {
        _scenarioStateStore = scenarioStateStore;
    }

    /// <inheritdoc />
    public override string Name => "scenario_hitcount";

    /// <inheritdoc />
    public override string FullName => "Scenario hit count";

    public override string[] Examples => new[] {$"(({Name}))", $"(({Name}:scenario name))"};

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub)
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
