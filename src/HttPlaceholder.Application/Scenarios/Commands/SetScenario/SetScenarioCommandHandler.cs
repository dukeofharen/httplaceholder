using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.SetScenario
{
    public class SetScenarioCommandHandler : IRequestHandler<SetScenarioCommand>
    {
        private readonly IScenarioService _scenarioService;

        public SetScenarioCommandHandler(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public Task<Unit> Handle(SetScenarioCommand request, CancellationToken cancellationToken)
        {
            _scenarioService.SetScenario(request.ScenarioName, request.ScenarioStateModel);
            return Unit.Task;
        }
    }
}
