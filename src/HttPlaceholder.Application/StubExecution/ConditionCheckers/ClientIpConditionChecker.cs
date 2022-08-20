using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using HttPlaceholder.Domain.Enums;
using NetTools;

namespace HttPlaceholder.Application.StubExecution.ConditionCheckers;

/// <summary>
/// Condition checker that verifies the client IP address. IP address can be both a single IP or an IP range.
/// </summary>
public class ClientIpConditionChecker : IConditionChecker
{
    private readonly IClientDataResolver _clientDataResolver;

    /// <summary>
    /// Constructs a <see cref="ClientIpConditionChecker"/> instance.
    /// </summary>
    public ClientIpConditionChecker(IClientDataResolver clientDataResolver)
    {
        _clientDataResolver = clientDataResolver;
    }

    /// <inheritdoc />
    public Task<ConditionCheckResultModel> ValidateAsync(StubModel stub)
    {
        var result = new ConditionCheckResultModel();
        var clientIpCondition = stub.Conditions?.ClientIp;
        if (clientIpCondition == null)
        {
            return Task.FromResult(result);
        }

        var clientIp = IPAddress.Parse(_clientDataResolver.GetClientIp());
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
