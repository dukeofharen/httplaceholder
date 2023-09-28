using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetAllScenarios;

/// <summary>
///     A query handler for retrieving all scenarios.
/// </summary>
public class GetAllScenariosQueryHandler : IRequestHandler<GetAllScenariosQuery, IEnumerable<ScenarioStateModel>>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="GetAllScenariosQueryHandler" /> instance.
    /// </summary>
    public GetAllScenariosQueryHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ScenarioStateModel>> Handle(GetAllScenariosQuery request,
        CancellationToken cancellationToken) =>
        await _stubContext.GetAllScenariosAsync(cancellationToken);
}
