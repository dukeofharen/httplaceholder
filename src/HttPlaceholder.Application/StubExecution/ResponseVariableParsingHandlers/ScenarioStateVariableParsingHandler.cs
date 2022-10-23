using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable handler that is used to insert the state of a specific scenario in the response
/// </summary>
internal class ScenarioStateVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IScenarioStateStore _scenarioStateStore;

    public ScenarioStateVariableParsingHandler(IScenarioStateStore scenarioStateStore, IFileService fileService) :
        base(fileService)
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
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken) =>
        Task.FromResult(matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => InsertState(current, match, stub)));

    private string InsertState(string current, Match match, StubModel stub)
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

        return current.Replace(match.Value, state);
    }
}
