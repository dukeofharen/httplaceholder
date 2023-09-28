using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetScenario;

/// <summary>
///     A query handler for retrieving a scenario.
/// </summary>
public class GetScenarioQueryHandler : IRequestHandler<GetScenarioQuery, ScenarioStateModel>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="GetScenarioQueryHandler" /> instance.
    /// </summary>
    public GetScenarioQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<ScenarioStateModel> Handle(GetScenarioQuery request, CancellationToken cancellationToken) =>
        await _stubContext.GetScenarioAsync(request.Scenario, cancellationToken)
            .IfNull(() => throw new NotFoundException($"Scenario with name '{request.Scenario}'."));
}
