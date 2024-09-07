using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands;

/// <summary>
///     A command for updating a stub.
/// </summary>
public class UpdateStubCommand(string stubId, StubModel stub) : IRequest<Unit>
{
    /// <summary>
    ///     Gets the stub ID to update.
    /// </summary>
    public string StubId { get; } = stubId;

    /// <summary>
    ///     Gets the stub to update.
    /// </summary>
    public StubModel Stub { get; } = stub;
}

/// <summary>
///     A command handler for updating a stub.
/// </summary>
public class UpdateStubCommandHandler(IStubContext stubContext, IStubModelValidator stubModelValidator)
    : IRequestHandler<UpdateStubCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(UpdateStubCommand request, CancellationToken cancellationToken)
    {
        var validationResults = stubModelValidator.ValidateStubModel(request.Stub).ToArray();
        if (validationResults.Length != 0)
        {
            throw new ValidationException(validationResults);
        }

        // Check that the stub is not read-only.
        var oldStub = await stubContext.GetStubAsync(request.StubId, cancellationToken)
            .IfNullAsync(() => throw new NotFoundException(nameof(StubModel), request.StubId));
        if (oldStub.Metadata?.ReadOnly == true)
        {
            throw new ValidationException(string.Format(StubResources.StubIsReadonly, request.StubId));
        }

        var newStub = await stubContext.GetStubAsync(request.Stub.Id, cancellationToken);
        if (newStub?.Metadata?.ReadOnly == true)
        {
            throw new ValidationException(string.Format(StubResources.StubIsReadonly, request.Stub.Id));
        }

        await stubContext.DeleteStubAsync(request.StubId, cancellationToken);
        await stubContext.DeleteStubAsync(request.Stub.Id, cancellationToken);
        await stubContext.AddStubAsync(request.Stub, cancellationToken);
        return Unit.Value;
    }
}
