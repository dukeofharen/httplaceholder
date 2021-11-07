using System.Collections.Generic;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetAllScenarios
{
    public class GetAllScenariosQuery : IRequest<IEnumerable<ScenarioStateModel>>
    {
    }
}
