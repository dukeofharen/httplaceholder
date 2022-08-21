using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.DeleteAllScenarios;

/// <summary>
/// A command handler for deleting all scenarios.
/// </summary>
public class DeleteAllScenariosCommandHandler : IRequestHandler<DeleteAllScenariosCommand>
{
    private readonly IScenarioService _scenarioService;

    /// <summary>
    /// Constructs a <see cref="DeleteAllScenariosCommandHandler"/> instance.
    /// </summary>
    public DeleteAllScenariosCommandHandler(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteAllScenariosCommand request, CancellationToken cancellationToken)
    {
        await _scenarioService.DeleteAllScenariosAsync();
        return Unit.Value;
    }
}
