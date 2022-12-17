using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert the display URL (so the full URL) in the response.
/// </summary>
internal class DisplayUrlResponseVariableParsingHandler : BaseVariableParsingHandler, ISingletonService
{
    private readonly IHttpContextService _httpContextService;
    private readonly ILogger<DisplayUrlResponseVariableParsingHandler> _logger;

    public DisplayUrlResponseVariableParsingHandler(IHttpContextService httpContextService, IFileService fileService,
        ILogger<DisplayUrlResponseVariableParsingHandler> logger) :
        base(fileService)
    {
        _httpContextService = httpContextService;
        _logger = logger;
    }

    /// <inheritdoc />
    public override string Name => "display_url";

    /// <inheritdoc />
    public override string FullName => "Display URL";

    /// <inheritdoc />
    public override string[] Examples => new[] {$"(({Name}))", $@"(({Name}:'\/users\/([0-9]{3})\/orders'))"};

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, Match[] matches, StubModel stub,
        CancellationToken cancellationToken) =>
        Task.FromResult(matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => HandleDisplayUrl(match, current, _httpContextService.DisplayUrl)));

    private string HandleDisplayUrl(Match match, string current, string url)
    {
        var result = string.Empty;
        if (match.Groups.Count != 3)
        {
            _logger.LogWarning(
                $"Number of regex matches for variable parser {GetType().Name} was {match.Groups.Count}, which should be 3.");
        }
        else if (string.IsNullOrWhiteSpace(match.Groups[2].Value))
        {
            result = url;
        }
        else
        {
            var regexValue = match.Groups[2].Value;
            try
            {
                var regex = new Regex(regexValue, RegexOptions.Multiline,
                    TimeSpan.FromSeconds(Constants.RegexTimeoutSeconds));
                var matches = regex.Matches(url).ToArray();
                if (matches.Length >= 1 && matches[0].Groups.Count > 1)
                {
                    result = matches[0].Groups[1].Value;
                }
                else
                {
                    _logger.LogInformation($"No result found in display URL for regular expression '{regexValue}'.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error occurred while executing regex '{regexValue}' on display URL.'");
            }
        }

        return current.Replace(match.Value, result);
    }
}
