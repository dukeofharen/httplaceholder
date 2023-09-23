using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.SetScenario;

/// <summary>
///     A command handler for setting a scenario.
/// </summary>
public class SetScenarioCommandHandler : IRequestHandler<SetScenarioCommand, Unit>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="SetScenarioCommandHandler" /> instance.
    /// </summary>
    public SetScenarioCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(SetScenarioCommand request, CancellationToken cancellationToken)
    {
        await _stubContext.SetScenarioAsync(request.ScenarioName, request.ScenarioStateModel, cancellationToken);
        return Unit.Value;
    }
}
