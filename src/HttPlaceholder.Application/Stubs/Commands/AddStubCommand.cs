using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands;

/// <summary>
///     A command for adding a stub.
/// </summary>
public class AddStubCommand(StubModel stub) : IRequest<FullStubModel>
{
    /// <summary>
    ///     Gets the stub to add.
    /// </summary>
    public StubModel Stub { get; } = stub;
}

/// <summary>
///     A command handler for adding a stub.
/// </summary>
public class AddStubCommandHandler(IStubContext stubContext, IStubModelValidator stubModelValidator)
    : IRequestHandler<AddStubCommand, FullStubModel>
{
    /// <inheritdoc />
    public async Task<FullStubModel> Handle(AddStubCommand request, CancellationToken cancellationToken)
    {
        var validationResults = stubModelValidator.ValidateStubModel(request.Stub).ToArray();
        // TODO
        if (validationResults.Any())
        {
            throw new ValidationException(validationResults);
        }

        // Delete stub with same ID.
        await stubContext.DeleteStubAsync(request.Stub.Id, cancellationToken);
        return await stubContext.AddStubAsync(request.Stub, cancellationToken);
    }
}
