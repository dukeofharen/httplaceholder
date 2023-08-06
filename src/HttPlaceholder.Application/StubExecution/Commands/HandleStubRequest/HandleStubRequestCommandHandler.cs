using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace HttPlaceholder.Application.StubExecution.Commands.HandleStubRequest;

/// <summary>
///     A command handler for handling a stub request.
/// </summary>
public class HandleStubRequestCommandHandler : IRequestHandler<HandleStubRequestCommand, Unit>
{
    private readonly IStubHandler _stubHandler;

    /// <summary>
    ///     Constructs a <see cref="HandleStubRequestCommandHandler"/> instance.
    /// </summary>
    /// <param name="stubHandler"></param>
    public HandleStubRequestCommandHandler(IStubHandler stubHandler)
    {
        _stubHandler = stubHandler;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(HandleStubRequestCommand request, CancellationToken cancellationToken)
    {
        await _stubHandler.HandleStubRequestAsync(cancellationToken);
        return Unit.Value;
    }
}
