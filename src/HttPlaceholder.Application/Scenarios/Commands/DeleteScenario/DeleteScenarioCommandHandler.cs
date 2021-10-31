using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.DeleteScenario
{
    public class DeleteScenarioCommandHandler : IRequestHandler<DeleteScenarioCommand>
    {
        private readonly IScenarioService _scenarioService;

        public DeleteScenarioCommandHandler(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public Task<Unit> Handle(DeleteScenarioCommand request, CancellationToken cancellationToken)
        {
            if (!_scenarioService.DeleteScenario(request.ScenarioName))
            {
                throw new NotFoundException($"Scenario '{request.ScenarioName}' not found.");
            }

            return Unit.Task;
        }
    }
}
