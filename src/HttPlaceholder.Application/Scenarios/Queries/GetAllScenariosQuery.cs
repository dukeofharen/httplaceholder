using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries;

/// <summary>
///     A query for retrieving all scenarios.
/// </summary>
public class GetAllScenariosQuery : IRequest<IEnumerable<ScenarioStateModel>>;

/// <summary>
///     A query handler for retrieving all scenarios.
/// </summary>
public class GetAllScenariosQueryHandler(IStubContext stubContext)
    : IRequestHandler<GetAllScenariosQuery, IEnumerable<ScenarioStateModel>>
{
    /// <inheritdoc />
    public async Task<IEnumerable<ScenarioStateModel>> Handle(GetAllScenariosQuery request,
        CancellationToken cancellationToken) =>
        await stubContext.GetAllScenariosAsync(cancellationToken);
}
