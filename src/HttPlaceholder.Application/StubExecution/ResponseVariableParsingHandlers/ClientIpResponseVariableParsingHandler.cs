using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Interfaces.Http;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert the client IP in the response.
/// </summary>
internal class ClientIpResponseVariableParsingHandler : IResponseVariableParsingHandler
{
    private readonly IClientDataResolver _clientDataResolver;

    public ClientIpResponseVariableParsingHandler(IClientDataResolver clientDataResolver)
    {
        _clientDataResolver = clientDataResolver;
    }

    /// <inheritdoc />
    public string Name => "client_ip";

    /// <inheritdoc />
    public string FullName => "Client IP";

    /// <inheritdoc />
    public string Example => "((client_ip))";

    /// <inheritdoc />
    public string Parse(string input, IEnumerable<Match> matches)
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
