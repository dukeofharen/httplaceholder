using System.Collections.Generic;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetAllScenarios;

/// <summary>
/// A query for retrieving all scenarios.
/// </summary>
public class GetAllScenariosQuery : IRequest<IEnumerable<ScenarioStateModel>>
{
}
