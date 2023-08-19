using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.StubExecution.Commands;

/// <summary>
///     A command handler that is used to execute the request against the registered stubs.
/// </summary>
public class HandleStubRequestCommandHandler : IRequestHandler<HandleStubRequestCommand, ResponseModel>
{
    private readonly IStubRequestExecutor _stubRequestExecutor;

    /// <summary>
    ///     Constructs a <see cref="HandleStubRequestCommandHandler" /> instance.
    /// </summary>
    /// <param name="stubRequestExecutor"></param>
    public HandleStubRequestCommandHandler(IStubRequestExecutor stubRequestExecutor)
    {
        _stubRequestExecutor = stubRequestExecutor;
    }

    /// <inheritdoc />
    public async Task<ResponseModel> Handle(HandleStubRequestCommand request, CancellationToken cancellationToken) =>
        await _stubRequestExecutor.ExecuteRequestAsync(cancellationToken);
}
