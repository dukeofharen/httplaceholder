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
///     Response variable parsing handler that is used to insert the root URL (so the URL without path + query string) in
///     the response.
/// </summary>
internal class RootUrlResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;

    public RootUrlResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService) :
        base(fileService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "root_url";

    /// <inheritdoc />
    public override string FullName => "Root URL";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))"};

    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub, CancellationToken cancellationToken)
    {
        var url = _httpContextService.RootUrl;
        return Task.FromResult(matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => current.Replace(match.Value, url)));
    }
}
