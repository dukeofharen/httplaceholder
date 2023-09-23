using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Scenarios.Commands.DeleteAllScenarios;

/// <summary>
///     A command handler for deleting all scenarios.
/// </summary>
public class DeleteAllScenariosCommandHandler : IRequestHandler<DeleteAllScenariosCommand, Unit>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    ///     Constructs a <see cref="DeleteAllScenariosCommandHandler" /> instance.
    /// </summary>
    public DeleteAllScenariosCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteAllScenariosCommand request, CancellationToken cancellationToken)
    {
        await _stubContext.DeleteAllScenariosAsync(cancellationToken);
        return Unit.Value;
    }
}
