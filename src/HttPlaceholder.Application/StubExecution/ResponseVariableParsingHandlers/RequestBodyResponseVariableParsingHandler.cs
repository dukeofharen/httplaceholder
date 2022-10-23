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
///     Response variable parsing handler that is used to insert the posted request body in the response.
/// </summary>
internal class RequestBodyResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;

    public RequestBodyResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService) :
        base(fileService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "request_body";

    /// <inheritdoc />
    public override string FullName => "Full request body";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))"};

    /// <inheritdoc />
    protected override async Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub, CancellationToken cancellationToken)
    {
        var body = await _httpContextService.GetBodyAsync(cancellationToken);
        return matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, body));
    }
}
