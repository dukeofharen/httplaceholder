﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HttPlaceholder.Application.Infrastructure.DependencyInjection;
using HttPlaceholder.Application.Interfaces.Http;
using HttPlaceholder.Common.Utilities;
using HttPlaceholder.Domain;
using Microsoft.Extensions.Logging;

namespace HttPlaceholder.Application.StubExecution.ResponseVariableParsingHandlers;

/// <summary>
///     Response variable parsing handler that is used to insert the display URL (so the full URL) in the response.
/// </summary>
internal class DisplayUrlResponseVariableParsingHandler(
    ILogger<DisplayUrlResponseVariableParsingHandler> logger,
    IUrlResolver urlResolver)
    : BaseVariableParsingHandler, ISingletonService
{
    /// <inheritdoc />
    public override string Name => "display_url";

    /// <inheritdoc />
    public override string FullName => ResponseVariableParsingResources.DisplayUrl;

    /// <inheritdoc />
    public override string[] Examples => [$"(({Name}))", $@"(({Name}:'\/users\/([0-9]{3})\/orders'))"];

    /// <inheritdoc />
    public override string GetDescription() => ResponseVariableParsingResources.DisplayUrlDescription;

    /// <inheritdoc />
    protected override Task<string> InsertVariablesAsync(string input, IEnumerable<Match> matches, StubModel stub,
        CancellationToken cancellationToken) =>
        matches
            .Where(match => match.Groups.Count >= 2)
            .Aggregate(input, (current, match) => HandleDisplayUrl(match, current, urlResolver.GetDisplayUrl()))
            .AsTask();

    private string HandleDisplayUrl(Match match, string current, string url)
    {
        var result = string.Empty;
        if (match.Groups.Count != 3)
        {
            logger.LogWarning(
                "Number of regex matches for variable parser {VariableParser} was {MatchCount}, which should be 3.",
                GetType().Name, match.Groups.Count);
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
                    logger.LogInformation("No result found in display URL for regular expression '{RegexValue}'.",
                        regexValue);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Error occurred while executing regex '{RegexValue}' on display URL.'",
                    regexValue);
            }
        }

        return current.Replace(match.Value, result);
    }
}
