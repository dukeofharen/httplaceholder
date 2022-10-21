using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand;

/// <summary>
///     A command handler for updating a stub.
/// </summary>
public class UpdateStubCommandHandler : IRequestHandler<UpdateStubCommand>
{
    private readonly IStubContext _stubContext;
    private readonly IStubModelValidator _stubModelValidator;

    /// <summary>
    ///     Constructs an <see cref="UpdateStubCommandHandler" /> instance.
    /// </summary>
    public UpdateStubCommandHandler(IStubContext stubContext, IStubModelValidator stubModelValidator)
    {
        _stubContext = stubContext;
        _stubModelValidator = stubModelValidator;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(UpdateStubCommand request, CancellationToken cancellationToken)
    {
        var validationResults = _stubModelValidator.ValidateStubModel(request.Stub).ToArray();
        if (validationResults.Any())
        {
            throw new ValidationException(validationResults);
        }

        // Check that the stub is not read-only.
        const string exceptionFormat = "Stub with ID '{0}' is read-only; it can not be updated through the API.";
        var oldStub = await _stubContext.GetStubAsync(request.StubId, cancellationToken)
            .IfNull(() => throw new NotFoundException(nameof(StubModel), request.StubId));
        if (oldStub.Metadata?.ReadOnly == true)
        {
            throw new ValidationException(string.Format(exceptionFormat, request.StubId));
        }

        var newStub = await _stubContext.GetStubAsync(request.Stub.Id, cancellationToken);
        if (newStub?.Metadata?.ReadOnly == true)
        {
            throw new ValidationException(string.Format(exceptionFormat, request.Stub.Id));
        }

        await _stubContext.DeleteStubAsync(request.StubId, cancellationToken);
        await _stubContext.DeleteStubAsync(request.Stub.Id, cancellationToken);
        await _stubContext.AddStubAsync(request.Stub, cancellationToken);

        return Unit.Value;
    }
}
