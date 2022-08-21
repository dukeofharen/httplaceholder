using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.SetScenario;

/// <summary>
/// A command handler for setting a scenario.
/// </summary>
public class SetScenarioCommandHandler : IRequestHandler<SetScenarioCommand>
{
    private readonly IScenarioService _scenarioService;

    /// <summary>
    /// Constructs a <see cref="SetScenarioCommandHandler"/> instance.
    /// </summary>
    public SetScenarioCommandHandler(IScenarioService scenarioService)
    {
        _scenarioService = scenarioService;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(SetScenarioCommand request, CancellationToken cancellationToken)
    {
        await _scenarioService.SetScenarioAsync(request.ScenarioName, request.ScenarioStateModel);
        return Unit.Value;
    }
}
