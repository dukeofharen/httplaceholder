using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries;

/// <summary>
///     A query for retrieving a scenario.
/// </summary>
public class GetScenarioQuery(string scenario) : IRequest<ScenarioStateModel>
{
    /// <summary>
    ///     Gets the scenario name.
    /// </summary>
    public string Scenario { get; } = scenario;
}

/// <summary>
///     A query handler for retrieving a scenario.
/// </summary>
public class GetScenarioQueryHandler(IStubContext stubContext) : IRequestHandler<GetScenarioQuery, ScenarioStateModel>
{
    /// <inheritdoc />
    public async Task<ScenarioStateModel> Handle(GetScenarioQuery request, CancellationToken cancellationToken) =>
        await stubContext.GetScenarioAsync(request.Scenario, cancellationToken)
            .IfNull(() => throw new NotFoundException($"Scenario with name '{request.Scenario}'."));
}
