using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using NetTools;
using static HttPlaceholder.Domain.ConditionCheckResultModel;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that verifies the client IP address. IP address can be both a single IP or an IP range.
/// </summary>
public class ClientIpConditionChecker(IClientDataResolver clientDataResolver) : BaseConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public override int Priority => 10;

    /// <inheritdoc />
    protected override bool ShouldBeExecuted(StubModel stub) => stub.Conditions?.ClientIp != null;

    /// <inheritdoc />
    protected override Task<ConditionCheckResultModel> PerformValidationAsync(StubModel stub,
        CancellationToken cancellationToken)
    {
        var clientIpCondition = stub.Conditions?.ClientIp;
        var clientIp = IPAddress.Parse(clientDataResolver.GetClientIp());
        var ranges = IPAddressRange.Parse(clientIpCondition).AsEnumerable();
        return ranges
            .Any(i => i.Equals(clientIp))
            ? ValidAsync()
            : InvalidAsync();
    }
}
