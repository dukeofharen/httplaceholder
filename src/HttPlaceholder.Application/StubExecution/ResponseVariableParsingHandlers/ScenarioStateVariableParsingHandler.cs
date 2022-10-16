using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable handler that is used to insert the state of a specific scenario in the response
/// </summary>
internal class ScenarioStateVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IScenarioStateStore _scenarioStateStore;

    public ScenarioStateVariableParsingHandler(IScenarioStateStore scenarioStateStore, IFileService fileService) : base(fileService)
    {
        _scenarioStateStore = scenarioStateStore;
    }

    /// <inheritdoc />
    public override string Name => "scenario_state";

    /// <inheritdoc />
    public override string FullName => "Scenario state";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))", $"(({Name}:scenario name))"};

    /// <inheritdoc />
    protected override string InsertVariables(string input, Match[] matches, StubModel stub)
    {
        foreach (var match in matches)
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
