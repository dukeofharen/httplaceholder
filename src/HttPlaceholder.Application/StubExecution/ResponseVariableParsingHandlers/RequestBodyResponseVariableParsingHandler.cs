using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert the posted request body in the response.
/// </summary>
internal class RequestBodyResponseVariableParsingHandler(
    IHttpContextService httpContextService,
    ILogger<RequestBodyResponseVariableParsingHandler> logger)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "request_body";

    /// <inheritdoc />
    public override string FullName => "Request body";

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}))", $"(({Name}:'key2=([a-z0-9]*)'))'"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.RequestBody;

    /// <inheritdoc />
    protected override async Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken)
    {
        var body = await httpContextService.GetBodyAsync(cancellationToken);
        return matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => HandleRequestBody(match, current, body));
    }

    private string HandleRequestBody(Match match, string current, string body)
    {
        var result = string.Empty;
        if (match.Groups.Count != 3)
        {
            logger.LogWarning(
                $"Number of regex matches for variable parser {GetType().Name} was {match.Groups.Count}, which should be 3.");
        }
        else if (string.IsNullOrWhiteSpace(match.Groups[2].Value))
        {
            result = body;
        }
        else
        {
            var regexValue = match.Groups[2].Value;
            try
            {
                var regex = new Regex(regexValue, RegexOptions.Multiline,
                    TimeSpan.FromSeconds(Constants.RegexTimeoutSeconds));
                var matches = regex.Matches(body).ToArray();
                if (matches.Length >= 1 && matches[0].Groups.Count > 1)
                {
                    result = matches[0].Groups[1].Value;
                }
                else
                {
                    logger.LogInformation($"No result found in request body for regular expression '{regexValue}'.");
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Error occurred while executing regex '{regexValue}' on request body.'");
            }
        }

        return current.Replace(match.Value, result);
    }
}
