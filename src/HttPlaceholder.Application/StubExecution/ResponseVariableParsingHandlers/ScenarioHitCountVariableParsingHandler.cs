using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable handler that is used to insert the hit count of a specific scenario in the response
/// </summary>
internal class ScenarioHitCountVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;
    private readonly IScenarioStateStore _scenarioStateStore;

    public ScenarioHitCountVariableParsingHandler(
        IScenarioStateStore scenarioStateStore,
        IFileService fileService,
        IHttpContextService httpContextService) :
        base(fileService)
    {
        _scenarioStateStore = scenarioStateStore;
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "scenario_hitcount";

    /// <inheritdoc />
    public override string FullName => "Scenario hit count";

    public override string[] Examples => new[] {$"(({Name}))", $"(({Name}:scenario name))"};

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken) =>
        Task.FromResult(matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => InsertHitCount(current, match, stub)));

    private string InsertHitCount(string current, Match match, StubModel stub)
    {
        int? hitCount = null;
        var customScenarioNameSet = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value);
        if (!customScenarioNameSet)
        {
            // Try to read the scenario state from the HttpContext as it contains the correct state of the moment the state was set.
            var state = _httpContextService.GetItem<ScenarioStateModel>(CachingKeys.ScenarioState);
            hitCount = state?.HitCount;
        }

        if (hitCount == null)
        {
            var scenarioName = StringHelper.GetFirstNonWhitespaceString(match.Groups[2].Value, stub.Scenario);
            if (!string.IsNullOrWhiteSpace(scenarioName))
            {
                var scenario = _scenarioStateStore.GetScenario(scenarioName);
                hitCount = scenario?.HitCount;
            }
        }

        var hitCountText = !hitCount.HasValue ? string.Empty : hitCount.Value.ToString();
        return current.Replace(match.Value, hitCountText);
    }
}
