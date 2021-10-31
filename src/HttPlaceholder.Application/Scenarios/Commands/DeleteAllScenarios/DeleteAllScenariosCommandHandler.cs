using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.DeleteAllScenarios
{
    public class DeleteAllScenariosCommandHandler : IRequestHandler<DeleteAllScenariosCommand>
    {
        private readonly IScenarioService _scenarioService;

        public DeleteAllScenariosCommandHandler(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        public Task<Unit> Handle(DeleteAllScenariosCommand request, CancellationToken cancellationToken)
        {
            _scenarioService.DeleteAllScenarios();
            return Unit.Task;
        }
    }
}
