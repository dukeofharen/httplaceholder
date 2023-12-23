using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert the client IP in the response.
/// </summary>
internal class ClientIpResponseVariableParsingHandler(IClientDataResolver clientDataResolver, IFileService fileService)
    : BaseVariableParsingHandler(fileService), ISingletonService
{
    /// <inheritdoc />
    public override string Name => "client_ip";

    /// <inheritdoc />
    public override string FullName => "Client IP";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))"};

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var ip = clientDataResolver.GetClientIp();
        return Task.FromResult(matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, ip)));
    }
}
