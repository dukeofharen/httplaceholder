using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands.DeleteStubsInTenant;

/// <summary>
/// A command handler for deleting all stubs belonging to a specific tenant.
/// </summary>
public class DeleteStubsInTenantCommandHandler : IRequestHandler<DeleteStubsInTenantCommand>
{
    private readonly IStubContext _stubContext;

    /// <summary>
    /// Constructs a <see cref="DeleteStubsInTenantCommandHandler"/> instance.
    /// </summary>
    /// <param name="stubContext"></param>
    public DeleteStubsInTenantCommandHandler(IStubContext stubContext)
    {
        _stubContext = stubContext;
    }

    /// <inheritdoc />
    public async Task<Unit> Handle(DeleteStubsInTenantCommand request, CancellationToken cancellationToken)
    {
        await _stubContext.DeleteAllStubsAsync(request.Tenant);
        return Unit.Value;
    }
}
