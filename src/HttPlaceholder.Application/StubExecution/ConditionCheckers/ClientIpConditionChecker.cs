using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using NetTools;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
///     Condition checker that verifies the client IP address. IP address can be both a single IP or an IP range.
/// </summary>
public class ClientIpConditionChecker(IClientDataResolver clientDataResolver) : IConditionChecker, ISingletonService
{
    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub, CancellationToken cancellationToken)
    {
        var result = new ConditionCheckResultModel();
        var clientIpCondition = stub.Conditions?.ClientIp;
        if (clientIpCondition == null)
        {
            return Task.FromResult(result);
        }

        var clientIp = IPAddress.Parse(clientDataResolver.GetClientIp());
        var ranges = IPAddressRange.Parse(clientIpCondition).AsEnumerable();
        result.ConditionValidation = ranges
            .Any(i => i.Equals(clientIp))
            ? ConditionValidationType.Valid
            : ConditionValidationType.Invalid;

        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public int Priority => 10;
}
