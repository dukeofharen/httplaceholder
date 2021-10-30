using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain.Entities;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Queries.GetAllScenarios
{
    public class GetAllScenariosQueryHandler : IRequestHandler<GetAllScenariosQuery, IEnumerable<ScenarioStateModel>>
    {
        private readonly IScenarioService _scenarioService;

        public GetAllScenariosQueryHandler(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public Task<IEnumerable<ScenarioStateModel>> Handle(GetAllScenariosQuery request,
            CancellationToken cancellationToken) =>
            Task.FromResult(_scenarioService.GetAllScenarios());
    }
}
