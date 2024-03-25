using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.StubExecution.Commands;

/// <summary>
///     A command that is used to execute the request against the registered stubs.
/// </summary>
public class HandleStubRequestCommand : IRequest<ResponseModel>;

/// <summary>
///     A command handler that is used to execute the request against the registered stubs.
/// </summary>
public class HandleStubRequestCommandHandler(IStubRequestExecutor stubRequestExecutor)
    : IRequestHandler<HandleStubRequestCommand, ResponseModel>
{
    /// <inheritdoc />
    public async Task<ResponseModel> Handle(HandleStubRequestCommand request, CancellationToken cancellationToken) =>
        await stubRequestExecutor.ExecuteRequestAsync(cancellationToken);
}
