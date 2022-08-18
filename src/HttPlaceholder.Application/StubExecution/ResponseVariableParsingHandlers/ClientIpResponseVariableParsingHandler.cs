using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert the client IP in the response.
/// </summary>
internal class ClientIpResponseVariableParsingHandler : BaseVariableParsingHandler
{
    private readonly IClientDataResolver _clientDataResolver;

    public ClientIpResponseVariableParsingHandler(IClientDataResolver clientDataResolver, IFileService fileService) :
        base(fileService)
    {
        _clientDataResolver = clientDataResolver;
    }

    /// <inheritdoc />
    public override string Name => "client_ip";

    /// <inheritdoc />
    public override string FullName => "Client IP";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))"};

    /// <inheritdoc />
    public override string Parse(string input, IEnumerable<Match> matches, StubModel stub)
    {
        var enumerable = matches as Match[] ?? matches.ToArray();
        if (!enumerable.Any())
        {
            return input;
        }

        var ip = _clientDataResolver.GetClientIp();
        return enumerable
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, ip));
    }
}
