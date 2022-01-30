using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Exceptions;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Stubs.Commands.UpdateStubCommand;

/// <summary>
/// A command handler for updating a stub.
/// </summary>
public class UpdateStubCommandHandler : IRequestHandler<UpdateStubCommand>
{
    private readonly IStubContext _stubContext;
    private readonly IStubModelValidator _stubModelValidator;

    /// <summary>
    /// Constructs an <see cref="UpdateStubCommandHandler"/> instance.
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

        // Delete stub with same ID.
        await _stubContext.DeleteStubAsync(request.StubId);
        await _stubContext.DeleteStubAsync(request.Stub.Id);

        await _stubContext.AddStubAsync(request.Stub);

        return Unit.Value;
    }
}
