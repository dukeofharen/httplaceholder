using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable handler that is used to insert the state of a specific scenario in the response
/// </summary>
internal class ScenarioStateVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IScenarioStateStore _scenarioStateStore;

    public ScenarioStateVariableParsingHandler(IScenarioStateStore scenarioStateStore)
    {
        _scenarioStateStore = scenarioStateStore;
    }

    /// <inheritdoc />
    public string Name => "scenario_state";

    /// <inheritdoc />
    public string FullName => "Scenario state";

    /// <inheritdoc />
    public string[] Examples => new[] {$"(({Name}))", $"(({Name}:scenario name))"};

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
            string state;
            var scenarioName = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value)
                ? match.Groups[2].Value
                : stub.Scenario ?? string.Empty;
            if (string.IsNullOrWhiteSpace(scenarioName))
            {
                state = string.Empty;
            }
            else
            {
                var scenario = _scenarioStateStore.GetScenario(scenarioName);
                state = scenario?.State;
            }

            input = input.Replace(match.Value, state);
        }

        return input;
    }
}
