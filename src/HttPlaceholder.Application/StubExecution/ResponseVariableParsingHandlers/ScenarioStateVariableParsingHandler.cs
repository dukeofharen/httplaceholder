﻿using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Entities;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable handler that is used to insert the state of a specific scenario in the response
/// </summary>
internal class ScenarioStateVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;
    private readonly IStubContext _stubContext;

    public ScenarioStateVariableParsingHandler(
        IStubContext stubContext,
        IFileService fileService,
        IHttpContextService httpContextService) :
        base(fileService)
    {
        _stubContext = stubContext;
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "scenario_state";

    /// <inheritdoc />
    public override string FullName => "Scenario state";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))", $"(({Name}:scenario name))"};

    /// <inheritdoc />
    protected override async Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var result = input;
        var filteredMatches = matches
            .Where(match => match.Groups.Count >= 2);
        foreach (var filteredMatch in filteredMatches)
        {
            result = await InsertStateAsync(result, filteredMatch, stub, cancellationToken);
        }

        return result;
    }

    private async Task<string> InsertStateAsync(string current, Match match, StubModel stub, CancellationToken cancellationToken)
    {
        var state = string.Empty;
        var customScenarioNameSet = match.Groups.Count == 3 && !string.IsNullOrWhiteSpace(match.Groups[2].Value);
        if (!customScenarioNameSet)
        {
            // Try to read the scenario state from the HttpContext as it contains the correct state of the moment the state was set.
            var scenarioState = _httpContextService.GetItem<ScenarioStateModel>(CachingKeys.ScenarioState);
            state = scenarioState?.State;
        }

        if (string.IsNullOrWhiteSpace(state))
        {
            var scenarioName = customScenarioNameSet
                ? match.Groups[2].Value
                : stub.Scenario ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(scenarioName))
            {
                var scenario = await _stubContext.GetScenarioAsync(scenarioName, cancellationToken);
                state = scenario?.State;
            }
        }

        return current.Replace(match.Value, state);
    }
}
