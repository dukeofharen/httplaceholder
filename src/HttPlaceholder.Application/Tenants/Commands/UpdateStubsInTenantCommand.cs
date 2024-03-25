using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.StubExecution;
using HttPlaceholder.Domain;
using MediatR;

namespace HttPlaceholder.Application.Tenants.Commands;

/// <summary>
///     A command for updating all stubs belonging to a tenant.
/// </summary>
public class UpdateStubsInTenantCommand(IEnumerable<StubModel> stubs, string tenant) : IRequest<Unit>
{
    /// <summary>
    ///     Gets the stubs that should be updated.
    /// </summary>
    public IEnumerable<StubModel> Stubs { get; } = stubs;

    /// <summary>
    ///     Gets the tenant.
    /// </summary>
    public string Tenant { get; } = tenant;
}

/// <summary>
///     A command handler for updating all stubs belonging to a tenant.
/// </summary>
public class UpdateStubsInTenantCommandHandler(IStubContext stubContext)
    : IRequestHandler<UpdateStubsInTenantCommand, Unit>
{
    /// <inheritdoc />
    public async Task<Unit> Handle(UpdateStubsInTenantCommand request, CancellationToken cancellationToken)
    {
        await stubContext.UpdateAllStubs(request.Tenant, request.Stubs, cancellationToken);
        return Unit.Value;
    }
}
