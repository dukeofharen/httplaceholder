using System.Collections.Generic;
using System.Text.RegularExpressions;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
/// Response variable parsing handler that is used to insert a request header in the response.
/// </summary>
internal class RequestHeaderResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;

    public RequestHeaderResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService) : base(fileService)
    {
        _httpContextService = httpContextService;
    }

    /// <inheritdoc />
    public override string Name => "request_header";

    /// <inheritdoc />
    public override string FullName => "Request header";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}:X-Api-Key))"};

    /// <inheritdoc />
    protected override string InsertVariables(string input, Match[] matches, StubModel stub)
    {
        var headers = _httpContextService.GetHeaders();
        foreach (var match in matches)
        {
            var headerName = match.Groups[2].Value;
            var replaceValue = headers.CaseInsensitiveSearch(headerName);

            input = input.Replace(match.Value, replaceValue);
        }

        return input;
    }
}
